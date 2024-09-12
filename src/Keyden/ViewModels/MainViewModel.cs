using Avalonia;

using Keyden.Views;

using Projektanker.Icons.Avalonia;

using ReactiveUI;

namespace Keyden.ViewModels;

public class MainViewModel : ViewModelBase
{
	public AgentK Kay { get; }
	public MainViewModel(AgentK kay)
	{
		Kay = kay;
	}

	public string Greeting => "Welcome to Avalonia!";

	private ObservableSshKey? _SelectedKey;
	public ObservableSshKey? SelectedKey
	{
		get => _SelectedKey;
		set
		{
			if (value == _SelectedKey)
				return;

			this.RaisePropertyChanging(nameof(SelectedKey));
			this.RaisePropertyChanging(nameof(IsKeySelected));
			this.RaisePropertyChanging(nameof(SelectedKeyName));
			this.RaisePropertyChanging(nameof(SelectedKeyDescriptor));
			_SelectedKey = value;
			this.RaisePropertyChanged(nameof(SelectedKey));
			this.RaisePropertyChanged(nameof(IsKeySelected));
			this.RaisePropertyChanged(nameof(SelectedKeyName));
			this.RaisePropertyChanged(nameof(SelectedKeyDescriptor));
		}
	}

	public readonly static DirectProperty<MainView, int> TabSelectedIndexProperty =
		AvaloniaProperty.RegisterDirect<MainView, int>(
			nameof(TabIndexSelected),
			o => (o.DataContext as MainViewModel)!.TabIndexSelected,
			(o, v) => (o.DataContext as MainViewModel)!.TabIndexSelected = v,
			defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);
	private int _TabIndexSelected = 0;
	public int TabIndexSelected
	{
		get => _TabIndexSelected;
		set
		{
			if (value == _TabIndexSelected)
				return;

			SelectedKey = null;

			this.RaisePropertyChanging(nameof(TabIndexSelected));
			this.RaisePropertyChanging(nameof(IsKeysTabSelected));
			this.RaisePropertyChanging(nameof(IsActivitySelected));
			_TabIndexSelected = value;
			this.RaisePropertyChanged(nameof(TabIndexSelected));
			this.RaisePropertyChanged(nameof(IsKeysTabSelected));
			this.RaisePropertyChanged(nameof(IsActivitySelected));
		}
	}
	public bool IsKeysTabSelected => TabIndexSelected is 0;
	public bool IsActivitySelected => TabIndexSelected is 1;

	public bool IsKeySelected => _SelectedKey is not null;

	public IconAnimation SyncingAnimation => IsSyncing ? IconAnimation.Spin : IconAnimation.None;
	private bool _IsSyncing;
	public bool IsSyncing
	{
		get => _IsSyncing;
		set
		{
			if (value == _IsSyncing)
				return;

			this.RaisePropertyChanging(nameof(IsSyncing));
			this.RaisePropertyChanging(nameof(SyncingAnimation));
			_IsSyncing = value;
			this.RaisePropertyChanged(nameof(IsSyncing));
			this.RaisePropertyChanged(nameof(SyncingAnimation));
			this.RaisePropertyChanged(nameof(NoKeysLoaded));
		}
	}
	public string SelectedKeyName => _SelectedKey?.Name ?? "Unknown Key";
	public string SelectedKeyDescriptor => _SelectedKey?.Fingerprint ?? "Unknown";

	public bool NoKeysLoaded => Kay.Keys.Count == 0;

	public async void SyncKeys()
	{
		IsSyncing = true;
		await Kay.SyncKeys();
		IsSyncing = false;
	}
}
