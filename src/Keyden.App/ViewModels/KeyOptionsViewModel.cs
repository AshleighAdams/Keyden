using ReactiveUI;

using System;
using System.Collections.Generic;

namespace Keyden.ViewModels;

public static class ModelTimeSpanExtensions
{
	public static readonly Dictionary<TimeSpan, string> NamedDurations;
	public static readonly Dictionary<string, TimeSpan> DurationsNamed;
	static ModelTimeSpanExtensions()
	{
		DurationsNamed = new()
		{
			["1 minute"] = TimeSpan.FromMinutes(1),
			["2 minutes"] = TimeSpan.FromMinutes(2),
			["5 minutes"] = TimeSpan.FromMinutes(5),
			["10 minutes"] = TimeSpan.FromMinutes(10),
			["30 minutes"] = TimeSpan.FromMinutes(30),
			["1 hour"] = TimeSpan.FromHours(1),
			["4 hours"] = TimeSpan.FromHours(4),
			["8 hours"] = TimeSpan.FromHours(8),
			["ever"] = TimeSpan.MaxValue,
		};
		NamedDurations = new();
		foreach (var (k, v) in DurationsNamed)
			NamedDurations[v] = k;
	}

	public static TimeSpan FromModelString(this string value) => DurationsNamed[value];
	public static string ToModelString(this TimeSpan value) => NamedDurations[value];
}

public class KeyOptionsViewModel : ViewModelBase
{
	private readonly string FallbackTimeSpan = "1 minute";
	public string[] KeepUnlockedDurations { get; } =
	[
		"1 minute",
		"2 minutes",
		"5 minutes",
		"10 minutes",
		"30 minutes",
		"1 hour",
		"4 hours",
		"8 hours",
		"ever",
	];
	public string[] InactivityDurations { get; } =
	[
		"1 minute",
		"2 minutes",
		"5 minutes",
		"10 minutes",
		"30 minutes",
		"1 hour",
		"4 hours",
		"8 hours",
	];

	public string MachineName => Environment.MachineName;

	public bool EnabledOnLocalMachine
	{
		get => Key?.EnableForMachines.Contains(MachineName) ?? false;
		set
		{
			if (EnabledOnLocalMachine)
				Key?.EnableForMachines.Remove(MachineName);
			else
				Key?.EnableForMachines.Add(MachineName);
		}
	}

	public bool EnabledOnAllMachines
	{
		get => Key?.EnableForMachines.Contains("*") ?? false;
		set
		{
			if (EnabledOnAllMachines)
				Key?.EnableForMachines.Remove("*");
			else
				Key?.EnableForMachines.Add("*");
		}
	}

	public bool RequireAuthorization
	{
		get => Key?.RequireAuthorization ?? true;
		set
		{
			if (Key is null)
				return;
			Key.RequireAuthorization = value;
		}
	}

	public bool RemainAuthorized
	{
		get => Key?.RemainAuthorized ?? false;
		set
		{
			if (Key is null)
				return;
			Key.RemainAuthorized = value;
		}
	}
	public string RemainAuthorizedFor
	{
		get => Key?.RemainAuthorizedFor.ToModelString() ?? FallbackTimeSpan;
		set
		{
			if (Key is null)
				return;
			Key.RemainAuthorizedFor = value.FromModelString();
		}
	}

	public bool RemainAuthorizedUntilKeyInactivity
	{
		get => Key?.RemainAuthorizedUntilKeyInactivity ?? false;
		set
		{
			if (Key is null)
				return;
			Key.RemainAuthorizedUntilKeyInactivity = value;
		}
	}
	public string RemainAuthorizedUntilKeyInactivityFor
	{
		get => Key?.RemainAuthorizedUntilKeyInactivityFor.ToModelString() ?? FallbackTimeSpan;
		set
		{
			if (Key is null)
				return;
			Key.RemainAuthorizedUntilKeyInactivityFor = value.FromModelString();
		}
	}

	public bool RemainAuthorizedUntilUserInactivity
	{
		get => Key?.RemainAuthorizedUntilUserInactivity ?? false;
		set
		{
			if (Key is null)
				return;
			Key.RemainAuthorizedUntilUserInactivity = value;
		}
	}
	public string RemainAuthorizedUntilUserInactivityFor
	{
		get => Key?.RemainAuthorizedUntilUserInactivityFor.ToModelString() ?? FallbackTimeSpan;
		set
		{
			if (Key is null)
				return;
			Key.RemainAuthorizedUntilUserInactivityFor = value.FromModelString();
		}
	}

	public bool RemainAuthorizedUntilLocked
	{
		get => Key?.RemainAuthorizedUntilLocked ?? false;
		set
		{
			if (Key is null)
				return;
			Key.RemainAuthorizedUntilLocked = value;
		}
	}

	public bool RequireAuthentication
	{
		get => Key?.RequireAuthentication ?? true;
		set
		{
			if (Key is null)
				return;
			Key.RequireAuthentication = value;
		}
	}

	public bool RemainAuthenticated
	{
		get => Key?.RemainAuthenticated ?? false;
		set
		{
			if (Key is null)
				return;
			Key.RemainAuthenticated = value;
		}
	}
	public string RemainAuthenticatedFor
	{
		get => Key?.RemainAuthenticatedFor.ToModelString() ?? FallbackTimeSpan;
		set
		{
			if (Key is null)
				return;
			Key.RemainAuthenticatedFor = value.FromModelString();
		}
	}

	public bool RemainAuthenticatedUntilKeyInactivity
	{
		get => Key?.RemainAuthenticatedUntilKeyInactivity ?? false;
		set
		{
			if (Key is null)
				return;
			Key.RemainAuthenticatedUntilKeyInactivity = value;
		}
	}
	public string RemainAuthenticatedUntilKeyInactivityFor
	{
		get => Key?.RemainAuthenticatedUntilKeyInactivityFor.ToModelString() ?? FallbackTimeSpan;
		set
		{
			if (Key is null)
				return;
			Key.RemainAuthenticatedUntilKeyInactivityFor = value.FromModelString();
		}
	}

	public bool RemainAuthenticatedUntilUserInactivity
	{
		get => Key?.RemainAuthenticatedUntilUserInactivity ?? false;
		set
		{
			if (Key is null)
				return;
			Key.RemainAuthenticatedUntilUserInactivity = value;
		}
	}
	public string RemainAuthenticatedUntilUserInactivityFor
	{
		get => Key?.RemainAuthenticatedUntilUserInactivityFor.ToModelString() ?? FallbackTimeSpan;
		set
		{
			if (Key is null)
				return;
			Key.RemainAuthenticatedUntilUserInactivityFor = value.FromModelString();
		}
	}

	public bool RemainAuthenticatedUntilLocked
	{
		get => Key?.RemainAuthenticatedUntilLocked ?? false;
		set
		{
			if (Key is null)
				return;
			Key.RemainAuthenticatedUntilLocked = value;
		}
	}

	private readonly string[] Properties =
	[
		nameof(Key),
		nameof(IsKeyNull),
		nameof(MachineName),
		nameof(EnabledOnLocalMachine),
		nameof(EnabledOnAllMachines),
		nameof(RequireAuthorization),
		nameof(RemainAuthorized),
		nameof(RemainAuthorizedFor),
		nameof(RemainAuthorizedUntilKeyInactivity),
		nameof(RemainAuthorizedUntilKeyInactivityFor),
		nameof(RemainAuthorizedUntilUserInactivity),
		nameof(RemainAuthorizedUntilUserInactivityFor),
		nameof(RemainAuthorizedUntilLocked),
		nameof(RequireAuthentication),
		nameof(RemainAuthenticated),
		nameof(RemainAuthenticatedFor),
		nameof(RemainAuthenticatedUntilKeyInactivity),
		nameof(RemainAuthenticatedUntilKeyInactivityFor),
		nameof(RemainAuthenticatedUntilUserInactivity),
		nameof(RemainAuthenticatedUntilUserInactivityFor),
		nameof(RemainAuthenticatedUntilLocked),
	];

	private ObservableSshKey? _Key;
	public ObservableSshKey? Key
	{
		get => _Key;
		set
		{
			if (Key == value)
				return;

			if (_Key is not null)
			{
				_Key.PropertyChanged -= Key_PropertyChanged;
				_Key.EnableForMachines.CollectionChanged -= EnableForMachines_CollectionChanged;
			}

			foreach (var property in Properties)
				this.RaisePropertyChanging(property);
			_Key = value;
			foreach (var property in Properties)
				this.RaisePropertyChanged(property);

			if (_Key is not null)
			{
				_Key.PropertyChanged += Key_PropertyChanged;
				_Key.EnableForMachines.CollectionChanged += EnableForMachines_CollectionChanged;
			}
		}
	}

	private void EnableForMachines_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
	{
		this.RaisePropertyChanged(nameof(EnabledOnAllMachines));
		this.RaisePropertyChanged(nameof(EnabledOnLocalMachine));
	}

	private void Key_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
	{
		this.RaisePropertyChanged(e.PropertyName);
	}

	public bool IsKeyNull => Key is null;
}

public class DesignKeyOptionsViewModel : KeyOptionsViewModel
{
	public DesignKeyOptionsViewModel()
	{
		Key = new ObservableSshKey(
			id: "test",
			name: "Test key",
			fingerprint: "Fingerdinger",
			publicKey: "ssh-des NUT5AAAAAB21zDNA");
		Key.SetOptions(new());
	}
}
