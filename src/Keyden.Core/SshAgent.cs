using Keyden.AgentProtocol;

using Renci.SshNet;
using Renci.SshNet.Security;
using Renci.SshNet.Security.Cryptography;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Keyden;

public record SshAgentOptions
{
}

public record struct SshKey
{
	public SshKey() { }
	public required string Id { get; init; }
	public string Name { get; set; } = string.Empty;
	public string Fingerprint { get; set; } = string.Empty;
	public ReadOnlyMemory<byte> PublicKey { get; set; } = Array.Empty<byte>();
	public string PublicKeyText { get; set; } = string.Empty;
	public ReadOnlyMemory<byte>? PrivateKey { get; set; } = Array.Empty<byte>();

	public bool IsEmpty => string.IsNullOrEmpty(Id);
}

public class ClientInfo
{
	public required string Username { get; set; }
	public required IReadOnlyList<Process> Processes { get; set; }

	public Process? MainProcess => Processes
		.Where(p => p.MainWindowHandle != IntPtr.Zero)
		.FirstOrDefault();

	public string ApplicationName => MainProcess?.ProcessName ?? "Unknown";
}


public interface ISshAgentHandler
{
	ValueTask<IReadOnlyList<SshKey>> GetPublicKeys(ClientInfo info, CancellationToken ct);
	ValueTask<SshKey> GetPrivateKey(ReadOnlyMemory<byte> publicKeyBlob, ClientInfo info, CancellationToken ct);

	string PipePath { get; }
	event EventHandler<EventArgs> PipePathChanged;
	Exception? ListenException { get; set; }
}

public class SshAgent
{
	private readonly SshAgentOptions Options;
	private readonly ISshAgentHandler Handler;
	private readonly ISystemServices SystemServices;

	public SshAgent(
		ISshAgentHandler handler,
		ISystemServices systemServices,
		SshAgentOptions? options = default)
	{
		Handler = handler;
		SystemServices = systemServices;
		Options = options ?? new();

		Handler.PipePathChanged += Handler_PipePathChanged;
		BeginConnection();
	}

	private static readonly AgentMessage FailureMessage = new()
	{
		Type = AgentMessageType.Failure,
		Contents = new(),
	};

	private CancellationTokenSource? CurrentListenCts { get; set; }
	private void Handler_PipePathChanged(object? sender, EventArgs e)
	{
		CurrentListenCts?.Cancel();
		BeginConnection();
	}

	private async void BeginConnection()
	{
		using var cts = new CancellationTokenSource();
		CurrentListenCts = cts;

		NamedPipeServerStream? pipeServer = null;
		try
		{
			var pipePath = Handler.PipePath;
			var pipePathRooted = Path.IsPathRooted(pipePath);

			pipeServer = new NamedPipeServerStream(
				pipeName: pipePath,
				direction: PipeDirection.InOut,
				maxNumberOfServerInstances: NamedPipeServerStream.MaxAllowedServerInstances,
				transmissionMode: PipeTransmissionMode.Byte,
				options: PipeOptions.Asynchronous | PipeOptions.WriteThrough | PipeOptions.CurrentUserOnly,
				inBufferSize: 5 * 1024,
				outBufferSize: 5 * 1024);

			Handler.ListenException = null;
			await pipeServer.WaitForConnectionAsync(cts.Token);
		}
		catch (Exception ex)
		{
			CurrentListenCts = null;
			Handler.ListenException = ex;
			return;
		}

		try
		{
			// listen for another another connection
			BeginConnection();

			await HandleConnection(pipeServer, cts);
		}
		catch (IOException) { }
		catch (TaskCanceledException) { }
		catch (OperationCanceledException) { }
		finally
		{
			cts.Cancel();
			pipeServer?.Dispose();
		}
	}

	private async Task HandleConnection(NamedPipeServerStream pipeServer, CancellationTokenSource cts)
	{
		var ct = cts.Token;

		await using var client = new AgentClient()
		{
			Stream = pipeServer,
		};

		var processes = pipeServer.GetParentProcesses(SystemServices);
		ClientInfo? clientInfo = null;

		var messageChannel = Channel.CreateUnbounded<AgentMessage>();
		_ = Task.Run(async () =>
		{
			try
			{
				while (true)
				{
					var msg = await client.ReadMessage(ct);
					messageChannel.Writer.TryWrite(msg);
				}
			}
			catch (IOException) { }
			finally
			{
				cts.Cancel();
			}
		});

		while (true)
		{
			var message = await messageChannel.Reader.ReadAsync(ct);

			var content = new BufferReader(message.Contents.ContiguousMemory);

			clientInfo ??= new ClientInfo()
			{
				Username = pipeServer.GetImpersonationUserName(),
				Processes = processes,
			};

			Console.WriteLine(message.Type);

			if (message.Type == AgentMessageType.TerminateConnection)
				break;

			switch (message.Type)
			{
				case AgentMessageType.RequestIdentities:
					{
						var keys = await Handler.GetPublicKeys(clientInfo, ct);

						var keysAnswer = new BufferWriter();
						keysAnswer.WriteUInt32((uint)keys.Count);

						foreach (var key in keys)
						{
							keysAnswer.WriteBlob(key.PublicKey.Span);
							keysAnswer.WriteString(key.Name);
						}

						await client.SendMessage(new()
						{
							Type = AgentMessageType.IdentitiesAnswer,
							Contents = keysAnswer.DataWritten,
						});
					}
					break;
				case AgentMessageType.SignRequest:
					{
						var publicKeyBlob = content.ReadBlob();
						var data = content.ReadBlob();
						var sigFlags = (SignatureFlags)content.ReadUInt32();

						var privateKey = await Handler.GetPrivateKey(publicKeyBlob, clientInfo, ct);
						if (privateKey is { IsEmpty: true } or { PrivateKey: null })
						{
							await client.SendMessage(FailureMessage);
							continue;
						}
						using var keyStream = new MemoryStream(privateKey.PrivateKey.Value.ToArray());
						using var privateSshFile = new PrivateKeyFile(keyStream);
						var privateSshKey = privateSshFile.Key;

						var algorithm = privateSshKey switch
						{
							RsaKey rsaKey => sigFlags switch
							{
								SignatureFlags.RsaSha512 => new KeyHostAlgorithm("rsa-sha2-512", privateSshKey, new RsaDigitalSignature(rsaKey, HashAlgorithmName.SHA512)),
								SignatureFlags.RsaSha256 => new KeyHostAlgorithm("rsa-sha2-256", privateSshKey, new RsaDigitalSignature(rsaKey, HashAlgorithmName.SHA256)),
								_ => null,
							},
							_ => privateSshFile.HostKeyAlgorithms.ToArray() switch
							{
								[HostAlgorithm only] => only,
								[HostAlgorithm first, ..] => null, // TODO: which one do we use?
								[] => null,
							},
						};

						if (algorithm is null)
						{
							await client.SendMessage(FailureMessage);
							continue;
						}

						var signed = algorithm.Sign(data.ToArray());

						var signResponse = new BufferWriter();
						signResponse.WriteBlob(signed);
						await client.SendMessage(new()
						{
							Type = AgentMessageType.SignResponse,
							Contents = signResponse.DataWritten,
						});
					}
					break;
				case AgentMessageType.Lock:
				case AgentMessageType.Unlock:
				case AgentMessageType.Extension:
				default:
					await client.SendMessage(FailureMessage);
					break;
			}
		}
	}

}
