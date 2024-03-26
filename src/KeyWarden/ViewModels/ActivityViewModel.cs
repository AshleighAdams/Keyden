
using DynamicData;

using System.Collections.ObjectModel;

namespace KeyWarden.ViewModels;

public enum ActivityImportance
{
	Normal,
	Warning,
	Critical,
}

public struct ActivityItem
{
	public required string Icon { get; set; }
	public required string Title { get; set; }
	public required string Description { get; set; }
	public required ActivityImportance Importance { get; set; }
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
			new (){ Icon = "fa-sync", Title = "Test", Description = "This is a test", Importance = ActivityImportance.Normal },
			new (){ Icon = "fa-sync", Title = "Test1", Description = "This is a test", Importance = ActivityImportance.Normal },
			new (){ Icon = "fa-sync", Title = "Test2", Description = "This is a test", Importance = ActivityImportance.Normal },
			new (){ Icon = "fa-sync", Title = "Test3", Description = "This is a test", Importance = ActivityImportance.Normal },
			new (){ Icon = "fa-sync", Title = "Test4", Description = "This is a test", Importance = ActivityImportance.Normal },
			new (){ Icon = "fa-sync", Title = "Test5", Description = "This is a test", Importance = ActivityImportance.Normal },
			new (){ Icon = "fa-sync", Title = "Test6", Description = "This is a test", Importance = ActivityImportance.Normal },
			new (){ Icon = "fa-sync", Title = "Test7", Description = "This is a test", Importance = ActivityImportance.Normal },
			new (){ Icon = "fa-sync", Title = "Test8", Description = "This is a test", Importance = ActivityImportance.Normal },
			new (){ Icon = "fa-sync", Title = "Test9", Description = "This is a test", Importance = ActivityImportance.Normal },
			new (){ Icon = "fa-sync", Title = "Testa", Description = "This is a test", Importance = ActivityImportance.Normal },
			new (){ Icon = "fa-sync", Title = "Testb", Description = "This is a test", Importance = ActivityImportance.Normal },
			new (){ Icon = "fa-sync", Title = "Testc", Description = "This is a test", Importance = ActivityImportance.Normal },
			new (){ Icon = "fa-sync", Title = "Testd", Description = "This is a test", Importance = ActivityImportance.Normal },
			new (){ Icon = "fa-sync", Title = "Teste", Description = "This is a test", Importance = ActivityImportance.Normal },
			new (){ Icon = "fa-sync", Title = "Testf", Description = "This is a test", Importance = ActivityImportance.Normal },
		]);
	}
}
