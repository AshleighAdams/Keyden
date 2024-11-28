
using DynamicData;

using System;
using System.Collections.ObjectModel;

namespace Keyden.ViewModels;

public enum ActivityImportance
{
	Normal,
	Warning,
	Critical,
}

public struct ActivityItem
{
	public ActivityItem() { }

	public required string Icon { get; set; }
	public required string Title { get; set; }
	public required string Description { get; set; }
	public required ActivityImportance Importance { get; set; }
	public DateTime When { get; set; } = DateTime.Now;
}

public class ActivityViewModel : ViewModelBase
{
	public AgentK Kay { get; }

	public ActivityViewModel()
	{
		Kay = App.GetService<AgentK>();

		Kay.NewActivity += Kay_NewActivity;
	}

	private void Kay_NewActivity(ActivityItem e)
	{
		if (Activities.Count >= 100)
			Activities.RemoveAt(0);
		Activities.Add(e);
	}

	public ObservableCollection<ActivityItem> Activities { get; } = [];
}


public class DesignActivityViewModel : ActivityViewModel
{
	public DesignActivityViewModel()
	{
		Activities.AddRange(
		[
			new (){ When = new(1998, 11, 19, 08, 47, 01), Icon = "fa-sync", Title = "Test", Description = "This is a test", Importance = ActivityImportance.Normal },
			new (){ When = new(2004, 11, 16, 02, 17, 02), Icon = "fa-sync", Title = "Test1", Description = "This is a really long test message to just try and see something about this bluh", Importance = ActivityImportance.Normal },
			new (){ When = new(2006, 06, 01, 06, 27, 03), Icon = "fa-sync", Title = "Test2", Description = "This is a test", Importance = ActivityImportance.Normal },
			new (){ When = new(2007, 10, 10, 08, 02, 04), Icon = "fa-sync", Title = "Test3", Description = "This is a test", Importance = ActivityImportance.Normal },
			new (){ When = new(2012, 09, 14, 08, 47, 21), Icon = "fa-sync", Title = "Test4", Description = "This is a test", Importance = ActivityImportance.Normal },
			new (){ When = new(2020, 03, 23, 16, 20, 05), Icon = "fa-sync", Title = "Test5", Description = "This is a test", Importance = ActivityImportance.Normal },
			new (){ When = new(2021, 11, 28, 12, 08, 53), Icon = "fa-sync", Title = "Test6", Description = "This is a test", Importance = ActivityImportance.Normal },
			new (){ When = new(2021, 12, 28, 11, 33, 53), Icon = "fa-sync", Title = "Test7", Description = "This is a test", Importance = ActivityImportance.Normal },
			new (){ When = new(2022, 11, 10, 09, 52, 53), Icon = "fa-sync", Title = "Test8", Description = "This is a test", Importance = ActivityImportance.Normal },
			new (){ When = new(2022, 10, 18, 07, 28, 53), Icon = "fa-sync", Title = "Test9", Description = "This is a test", Importance = ActivityImportance.Normal },
			new (){ When = new(2023, 09, 27, 15, 45, 37), Icon = "fa-sync", Title = "Testa", Description = "This is a test", Importance = ActivityImportance.Normal },
			new (){ When = new(2023, 11, 28, 07, 02, 53), Icon = "fa-sync", Title = "Testb", Description = "This is a test", Importance = ActivityImportance.Normal },
			new (){ When = new(2024, 10, 30, 07, 58, 00), Icon = "fa-sync", Title = "Testc", Description = "This is a test", Importance = ActivityImportance.Normal },
			new (){ When = new(2024, 11, 27, 22, 32, 34), Icon = "fa-sync", Title = "Testd", Description = "This is a test", Importance = ActivityImportance.Normal },
			new (){ When = new(2024, 11, 28, 07, 00, 01), Icon = "fa-sync", Title = "Teste", Description = "This is a test", Importance = ActivityImportance.Normal },
			new (){ When = new(2024, 11, 28, 07, 02, 53), Icon = "fa-sync", Title = "Testf", Description = "This is a test", Importance = ActivityImportance.Normal },
		]);
	}
}
