using System;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Runtime.Intrinsics;
using System.Text;

namespace KeyWarden.SshAgent;

internal static class AgentProtocolPrimitiveExtensions
{
	public static byte ReadByte(this BufferReader self)
	{
		return self.NextBlock(1).Span[0];
	}
	public static void WriteByte(this BufferWriter self, byte value)
	{
		self.NextBlock(1).Span[0] = value;
	}

	public static bool ReadBool(this BufferReader self)
	{
		return self.ReadByte() != 0;
	}
	public static void WriteBool(this BufferWriter self, bool value)
	{
		self.WriteByte(value ? (byte)1 : (byte)0);
	}

	public static uint ReadUInt32(this BufferReader self)
	{
		var block = self.NextBlock(sizeof(uint));
		return BinaryPrimitives.ReadUInt32BigEndian(block.Span);
	}
	public static void WriteUInt32(this BufferWriter self, uint value)
	{
		var block = self.NextBlock(sizeof(uint));
		BinaryPrimitives.WriteUInt32BigEndian(block.Span, value);
	}

	public static string ReadString(this BufferReader self)
	{
		var length = (int)self.ReadUInt32();
		if (length == 0)
			return string.Empty;

		var valueBlock = self.NextBlock(length);

		return Utf8.GetString(valueBlock.Span);
	}
	private static readonly UTF8Encoding Utf8 = new UTF8Encoding(false, false);
	public static void WriteString(this BufferWriter self, string value)
	{
		var encodedLength = Utf8.GetByteCount(value);
		self.WriteUInt32((uint)encodedLength);

		var valueBlock = self.NextBlock(encodedLength);
		var bytesWritten = Utf8.GetBytes(value, valueBlock.Span);

		Debug.Assert(bytesWritten == encodedLength);
	}

	public static ReadOnlyMemory<byte> ReadBlob(this BufferReader self)
	{
		var length = (int)self.ReadUInt32();
		return self.NextBlock(length);
	}
	public static void WriteBlob(this BufferWriter self, ReadOnlySpan<byte> value)
	{
		self.WriteUInt32((uint)value.Length);
		var block = self.NextBlock(value.Length);
		value.CopyTo(block.Span);
	}
}
