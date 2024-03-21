using System;

namespace KeyWarden.SshAgent;

internal class BufferReader
{
	public BufferReader(ReadOnlyMemory<byte> buffer)
	{
		Buffer = buffer;
	}

	public int Position { get; set; }
	private readonly ReadOnlyMemory<byte> Buffer;

	public ReadOnlyMemory<byte> NextBlock(int length)
	{
		if (Position + length >= Buffer.Length)
			throw new ArgumentOutOfRangeException(nameof(length));

		var position = Position;
		Position += length;

		return Buffer.Slice(position, length);
	}
}
