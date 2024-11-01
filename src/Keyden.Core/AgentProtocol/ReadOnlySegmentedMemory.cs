using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Keyden.AgentProtocol;

internal struct ReadOnlySegmentedMemory<T> : IEnumerable<ReadOnlyMemory<T>>
{
	private bool SingleSegment { get; set; } = true;
	[MemberNotNullWhen(true, nameof(SingleSegment))]
	private ReadOnlyMemory<T>? Segment { get; }
	[MemberNotNullWhen(false, nameof(SingleSegment))]
	private ReadOnlyMemory<T>[]? Segments { get; }
	public int Length { get; }

	public ReadOnlyMemory<T> ContiguousMemory
	{
		get
		{
			if (!SingleSegment)
				throw new NotSupportedException("Memory with multiple segments is not supported");
			return Segment!.Value;
		}
	}

	public ReadOnlySegmentedMemory()
	{
		SingleSegment = true;
		Segment = Array.Empty<T>();
		Length = 0;
	}
	public ReadOnlySegmentedMemory(ReadOnlyMemory<T> segment)
	{
		SingleSegment = true;
		Segment = segment;
		Length = segment.Length;
	}
	public ReadOnlySegmentedMemory(params ReadOnlyMemory<T>[] segments)
	{
		SingleSegment = false;
		Segments = segments;
		foreach (var segment in Segments)
			Length += segment.Length;
	}

	public void CopyTo(Memory<T> destination)
	{
		if (SingleSegment)
		{
			Segment!.Value.CopyTo(destination);
		}
		else
			foreach (var segment in Segments!)
			{
				segment.CopyTo(destination);
				destination = destination.Slice(segment.Length);
			}
	}

	public IEnumerator<ReadOnlyMemory<T>> GetEnumerator()
	{
		return new Enumerator(this);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return new Enumerator(this);
	}

	public struct Enumerator : IEnumerator<ReadOnlyMemory<T>>
	{
		private ReadOnlySegmentedMemory<T> Self;
		private int Position;
		internal Enumerator(ReadOnlySegmentedMemory<T> self)
		{
			Self = self;
		}

		public ReadOnlyMemory<T> Current => Self.SingleSegment ? Self.Segment!.Value : Self.Segments![Position];
		object IEnumerator.Current => (this as IEnumerator<T>)!.Current!;

		public void Dispose()
		{
		}

		public bool MoveNext()
		{
			Position++;

			if (Self.SingleSegment)
				return false;

			return Position < Self.Segments!.Length;
		}

		public void Reset()
		{
			Position = 0;
		}
	}
}
