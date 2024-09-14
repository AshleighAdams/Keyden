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

internal static partial class Unix
{
	[LibraryImport("libc", EntryPoint = "getuid", SetLastError = true)]
	public static partial uint Getuid();
}

public record SshAgentOptions
{
	public static string DefaultPipePath
	{
		get
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				return "openssh-ssh-agent";

			if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
			{
				var uid = Unix.Getuid().ToString(CultureInfo.InvariantCulture);
				var socketDirectory = $"/run/user/{uid}";

				// check for legacy linux systems
				if (!Directory.Exists(socketDirectory))
					socketDirectory = Directory.Exists("/run") ? "/run" : "/var/run";

				if (!Directory.Exists(socketDirectory))
					return "keyden-ssh-agent";

				socketDirectory += "/keyden";
				var socketPath = $"{socketDirectory}/ssh-agent.sock";

				if (!Directory.Exists(socketDirectory))
					Directory.CreateDirectory(socketDirectory);
				return socketPath;
			}

			if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{
				var uid = Unix.Getuid().ToString(CultureInfo.InvariantCulture);
				return $"/var/run/{uid}-keyden-ssh-agent.sock";
			}

			return "keyden-ssh-agent";
		}
	}

	public string PipeName { get; init; } = "openssh-ssh-agent";
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
}

public class SshAgent
{
	private readonly SshAgentOptions Options;
	private readonly ISshAgentHandler Handler;

	public SshAgent(
		ISshAgentHandler handler,
		SshAgentOptions? options = default)
	{
		Handler = handler;
		Options = options ?? new();

		BeginConnection();
	}

	private static readonly AgentMessage FailureMessage = new()
	{
		Type = AgentMessageType.Failure,
		Contents = new(),
	};


	private async void BeginConnection()
	{
		using var cts = new CancellationTokenSource();

		using var pipeServer = new NamedPipeServerStream(
			pipeName: Options.PipeName,
			direction: PipeDirection.InOut,
			maxNumberOfServerInstances: NamedPipeServerStream.MaxAllowedServerInstances,
			transmissionMode: PipeTransmissionMode.Byte,
			options: PipeOptions.Asynchronous | PipeOptions.WriteThrough | PipeOptions.CurrentUserOnly,
			inBufferSize: 5 * 1024,
			outBufferSize: 5 * 1024);

		await pipeServer.WaitForConnectionAsync();

		// listen for another another connection
		BeginConnection();

		try
		{
			await HandleConnection(pipeServer, cts);
		}
		catch (IOException) { }
		catch (TaskCanceledException) { }
		catch (OperationCanceledException) { }
		finally
		{
			cts.Cancel();
		}
	}

	private async Task HandleConnection(NamedPipeServerStream pipeServer, CancellationTokenSource cts)
	{
		var ct = cts.Token;

		await using var client = new AgentClient()
		{
			Stream = pipeServer,
		};

		var processes = pipeServer.GetParentProcesses();
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
