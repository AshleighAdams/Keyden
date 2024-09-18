using System;
using System.Collections.Generic;

namespace Keyden;

public static class LinqExtensions
{
	public static IEnumerable<T> Until<T>(this IEnumerable<T> source, Func<T, bool> predicate)
	{
		foreach (var item in source)
		{
			if (predicate(item))
				yield break;
			yield return item;
		}
	}
	public static IEnumerable<string> CompactDuplicates(this IEnumerable<string> source)
	{
		var it = source.GetEnumerator();

		var movedNext = it.MoveNext();
		while (movedNext)
		{
			var item = it.Current;
			var count = 0;

			while (movedNext && it.Current == item)
			{
				count++;
				movedNext = it.MoveNext();
			}

			if (count == 1)
				yield return item;
			else
				yield return $"{item} (x{count})";

		}
	}
}
