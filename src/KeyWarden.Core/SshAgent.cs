using KeyWarden.AgentProtocol;

using Microsoft.DevTunnels.Ssh;
using Microsoft.DevTunnels.Ssh.Algorithms;

using System;
using System.Buffers.Binary;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using SshBuffer = Microsoft.DevTunnels.Ssh.Buffer;

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

		var pipeServer = new NamedPipeServerStream(
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

		await using var client = new AgentClient()
		{
			Stream = pipeServer,
		};

		var processes = new List<Process>();
		var p = pipeServer.GetProcess();

		while (p is not null)
		{
			processes.Add(p);
			p = p.GetParent();
		}

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
						var keys = await Handler.GetPublicKeys(clientInfo, cts.Token);

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
						var privateKey = await Handler.GetPrivateKey(publicKey, clientInfo, cts.Token);
						if (privateKey is { IsEmpty: true } or { PrivateKey: null })
						{
							await client.SendMessage(FailureMessage);
							continue;
						}
						using var keyPair = KeyPair.ImportKeyBytes(privateKey.PrivateKey.Value.ToArray());
						
						// figure out the key type
						int length = (int)BinaryPrimitives.ReadUInt32BigEndian(publicKeyBlob.Span.Slice(0, sizeof(uint)));
						var keyType = publicKeyBlob.Slice(sizeof(uint), length);

						if (keyPair is Rsa.KeyPair rsaKeyPair)
						{
							var signResponse = new BufferWriter();
							using var rsaAlg = RSAOpenSsl.Create(rsaKeyPair.ExportParameters(includePrivate: true));

							HashAlgorithmName hashAlg;
							string hashAlgStr;
							if (sigFlags.HasFlag(SignatureFlags.RsaSha512))
							{
								hashAlg = HashAlgorithmName.SHA512;
								hashAlgStr = "rsa-sha2-512";
							}
							else if (sigFlags.HasFlag(SignatureFlags.RsaSha256))
							{
								hashAlg = HashAlgorithmName.SHA256;
								hashAlgStr = "rsa-sha2-256";
							}
							else
							{
								await client.SendMessage(FailureMessage);
								continue;
							}

							var signature = rsaAlg.SignData(data.Span, hashAlg, RSASignaturePadding.Pkcs1);

							var formattedSig = new BufferWriter();
							formattedSig.WriteString(hashAlgStr);
							formattedSig.WriteBlob(signature);

							signResponse.WriteUInt32((uint)formattedSig.Length);
							var sigBlock = signResponse.NextBlock(formattedSig.Length);
							formattedSig.DataWritten.CopyTo(sigBlock);

							await client.SendMessage(new()
							{
								Type = AgentMessageType.SignResponse,
								Contents = signResponse.DataWritten,
							});
						}
						else
						{
							await client.SendMessage(FailureMessage);
							continue;
						}
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
