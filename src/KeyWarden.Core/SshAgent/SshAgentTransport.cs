using System;
using System.Buffers.Binary;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace KeyWarden.SshAgent;

// https://datatracker.ietf.org/doc/html/draft-miller-ssh-agent#name-private-key-operations
// https://www.rfc-editor.org/rfc/rfc4251.html
internal enum AgentMessageType : byte
{
	// client to agent
	RequestIdentities = 11,
	SignRequest = 13,
	AddIdentity = 17,
	RemoveIdentity = 18,
	RemoveAllIdentities = 19,
	AddIdConstrained = 25,
	AddSmartcard_Key = 20,
	RemoveSmartcard_Key = 21,
	Lock = 22,
	Unlock = 23,
	AddSmartcardKeyConstrained = 26,
	Extension =  27,

	// agent to client
	Failure = 5,
	Success = 6,
	ExtensionFailure = 28,
	IdentitiesAnswer = 12,
	SignResponse = 14,
}

internal enum Constraints : byte
{
	Lifetime = 1,
	Confirm = 2,
	Extension = 255,
}

[Flags]
internal enum SignatureFlags : byte
{
	Reserved = 1,
	RsaSha2_256 = 2,
	RsaSha2_512 = 4,
}

internal record struct AgentMessage
{
	public required AgentMessageType Type { get; set; }
	public required ReadOnlySegmentedMemory<byte> Contents { get; set; }
}

internal sealed class SshAgentTransport
{
	public required Stream Stream { get; init; }
	public int MaxContentLength { get; set; } = 1024 * 5; // copies the OpenSSH portable max

	public async Task<AgentMessage> ReadMessage(CancellationToken ct = default)
	{
		const int headerLength = 5;
		var headerBytes = new byte[headerLength];
		await Stream.ReadAsync(headerBytes, 0, headerBytes.Length, ct);

		var length = BinaryPrimitives.ReadUInt32BigEndian(headerBytes.AsSpan(0,4));
		var type = headerBytes[4];

		var contentLength = length - 1;
		if (contentLength < 0 || contentLength >= MaxContentLength)
			throw new Exception($"Agent message content length of {length} is out of range");

		Memory<byte> contents = Array.Empty<byte>();
		if (contentLength > 0)
		{
			var contentsBuffer = new byte[contentLength];
			await Stream.ReadAsync(contentsBuffer, 0, (int)contentLength, ct);
			contents = contentsBuffer;
		}

		return new AgentMessage()
		{
			Type = (AgentMessageType)type,
			Contents = new(contents),
		};
	}

	public async Task SendMessage(AgentMessage message, CancellationToken ct = default)
	{
		const int headerLength = 5;
		var contentLength = message.Contents.Length;
		var length = message.Contents.Length + 1;
		var buffer = new byte[headerLength + contentLength];

		BinaryPrimitives.WriteUInt32BigEndian(buffer.AsSpan(0, 4), (uint)length);
		buffer[4] = (byte)message.Type;

		if (contentLength > 0)
			message.Contents.CopyTo(buffer.AsMemory().Slice(headerLength));

		await Stream.WriteAsync(buffer, ct);
		await Stream.FlushAsync();
	}
}
