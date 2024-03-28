
using ReactiveUI;

namespace KeyWarden.ViewModels;

public class KeyOptionsViewModel : ViewModelBase
{
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

	private ObservableSshKey? _Key;
	public ObservableSshKey? Key
	{
		get => _Key;
		set
		{
			if (Key == value)
				return;
			this.RaisePropertyChanging(nameof(Key));
			this.RaisePropertyChanging(nameof(IsKeyNull));
			_Key = value;
			this.RaisePropertyChanged(nameof(Key));
			this.RaisePropertyChanged(nameof(IsKeyNull));
		}
	}

	public bool IsKeyNull => Key is null;
}

public class DesignKeyOptionsViewModel : KeyOptionsViewModel
{
	public DesignKeyOptionsViewModel()
	{
		Key = new ObservableSshKey()
		{
			Name = "Test key",
			Fingerprint = "Fingerdinger",
			PublicKey = "ssh-des NUT5AAAAAB21zDNA",
		};
	}
}
