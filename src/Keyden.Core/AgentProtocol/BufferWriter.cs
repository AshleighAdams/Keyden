using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;

namespace Keyden.AgentProtocol;

internal class BufferWriter
{
	public BufferWriter()
	{
	}

	public int Length { get; set; }

	private record Segment
	{
		public int Position;
		public Memory<byte> Memory = Array.Empty<byte>();
	}

	private readonly List<Segment> Segments = new();
	private int NextAllocationSize = 128;
	private void Allocate(int minLength)
	{
		NextAllocationSize = Math.Max(minLength, NextAllocationSize);
		Segments.Add(new Segment() { Memory = new byte[NextAllocationSize] });
		NextAllocationSize = NextAllocationSize + NextAllocationSize / 2;
	}

	private void Prepare(int minLength)
	{
		if (Segments.Count == 0)
		{
			Allocate(minLength);
			return;
		}

		var currentSegment = Segments[^1];
		var remaining = currentSegment.Memory.Length - currentSegment.Position;
		if (remaining >= minLength)
			return;

		Allocate(minLength);
	}

	public Memory<byte> NextBlock(int length)
	{
		Prepare(length);
		var currentSegment = Segments[^1];
		var position = currentSegment.Position;

		currentSegment.Position += length;
		Length += length;

		return currentSegment.Memory.Slice(position, length);
	}

	public Memory<byte> LeaseBlock(int minimumLength = 0)
	{
		Prepare(minimumLength);
		var currentSegment = Segments[^1];

		return currentSegment.Memory.Slice(currentSegment.Position);
	}

	public ReadOnlySegmentedMemory<byte> DataWritten
	{
		get
		{
			var segments = Segments.Select(segment => (ReadOnlyMemory<byte>)segment.Memory.Slice(0, segment.Position)).ToArray();
			return new ReadOnlySegmentedMemory<byte>(segments);
		}
	}
}
