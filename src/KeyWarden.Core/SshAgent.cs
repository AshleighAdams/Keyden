using KeyWarden.AgentProtocol;

using Renci.SshNet;
using Renci.SshNet.Security;
using Renci.SshNet.Security.Cryptography;

using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace KeyWarden;

public record SshAgentOptions
{
	public string PipeName { get; init; } = "openssh-ssh-agent";
}

public record struct SshKey
{
	private string _Name;
	public string Name
	{
		get => _Name ?? string.Empty;
		set => _Name = value;
	}

	public ReadOnlyMemory<byte> PublicKey { get; set; }
	public ReadOnlyMemory<byte>? PrivateKey { get; set; }
	public bool IsEmpty => _Name is null;
}

public class ClientInfo
{
	public required string Username { get; set; }
	public required IReadOnlyList<Process> Processes { get; set; }
}


public interface ISshAgentHandler
{
	ValueTask<IReadOnlyList<SshKey>> GetPublicKeys(ClientInfo info, CancellationToken ct);
	ValueTask<SshKey> GetPrivateKey(SshKey publicKey, ClientInfo info, CancellationToken ct);
}

public class SshAgent
{
	private readonly SshAgentOptions Options;
	private readonly ISshAgentHandler Handler;

	public SshAgent(ISshAgentHandler handler, SshAgentOptions? options = default)
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
		var cts = new CancellationTokenSource();

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
			await HandleConnection(pipeServer, cts.Token);
		}
		catch (EndOfStreamException)
		{
		}
		finally
		{
			cts.Cancel();
		}
	}

	private async Task HandleConnection(NamedPipeServerStream pipeServer, CancellationToken ct)
	{
		await using var client = new AgentClient()
		{
			Stream = pipeServer,
		};

		var processes = pipeServer.GetParentProcesses();
		ClientInfo? clientInfo = null;

		while (true)
		{
			var message = await client.ReadMessage();
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

						var publicKey = new SshKey()
						{
							PublicKey = publicKeyBlob,
						};
						var privateKey = await Handler.GetPrivateKey(publicKey, clientInfo, ct);
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
