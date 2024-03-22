using KeyWarden.AgentProtocol;

using System;

namespace KeyWarden;

using SshBuffer = Microsoft.DevTunnels.Ssh.Buffer;

internal unsafe static partial class SshBufferExtensions
{
	public static SshBuffer ToSshBuffer(this Memory<byte> self)
	{
		return SshBuffer.From(self.ToArray());
	}
	public static SshBuffer ToSshBuffer(this ReadOnlyMemory<byte> self)
	{
		return SshBuffer.From(self.ToArray());
	}


	public static SshBuffer ReadBlobSshBuffer(this BufferReader self)
	{
		var length = (int)self.ReadUInt32();
		var buffer = new SshBuffer(length);
		self.NextBlock(length).CopyTo(buffer.Memory);
		return buffer;
	}
	public static void WriteBlobSshBuffer(this BufferWriter self, SshBuffer value)
	{
		self.WriteBlob(value.Span);
	}
}
