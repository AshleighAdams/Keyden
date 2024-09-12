using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Keyden;

public class SortableObservableCollection<T, TCompare> : ObservableCollection<T>
{
	public required Func<T, TCompare> SortingSelector { get; set; }
	public bool Descending { get; set; }
	protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
	{
		base.OnCollectionChanged(e);

		if (SortingSelector is null || e.Action is NotifyCollectionChangedAction.Remove or NotifyCollectionChangedAction.Reset)
			return;

		var query = this
		  .Select((item, index) => (Item: item, Index: index));
		query = Descending
		  ? query.OrderByDescending(tuple => SortingSelector(tuple.Item))
		  : query.OrderBy(tuple => SortingSelector(tuple.Item));

		var map = query
			.Select((tuple, index) => (OldIndex: tuple.Index, NewIndex: index))
			.Where(o => o.OldIndex != o.NewIndex);

		using var enumerator = map.GetEnumerator();
		if (enumerator.MoveNext())
			Move(enumerator.Current.OldIndex, enumerator.Current.NewIndex);
	}
}
