
using ReactiveUI;

namespace KeyWarden.ViewModels;

public class KeyOptionsViewModel : ViewModelBase
{
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
