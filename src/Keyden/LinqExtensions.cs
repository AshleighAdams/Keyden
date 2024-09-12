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
}
