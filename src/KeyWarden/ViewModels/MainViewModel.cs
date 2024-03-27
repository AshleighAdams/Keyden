using CommunityToolkit.Mvvm.ComponentModel;

using DynamicData;

using Projektanker.Icons.Avalonia;

using ReactiveUI;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KeyWarden.ViewModels;

public partial class ObservableSshKey : ObservableObject
{
	[ObservableProperty]
	private string _Name = string.Empty;

	[ObservableProperty]
	private string _Fingerprint = string.Empty;

	[ObservableProperty]
	private string _PublicKey = string.Empty;

	[ObservableProperty]
	private string _PrivateKey = string.Empty;
}

public class MainViewModel : ViewModelBase
{
	private ISshKeyStore KeyStore { get; }
	public MainViewModel(ISshKeyStore keyStore)
	{
		KeyStore = keyStore;
	}

	public string Greeting => "Welcome to Avalonia!";
	public ObservableCollection<ObservableSshKey> Keys { get; } = new();
	public string KeyCount => $"{Keys.Count} Keys";

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

	private int _TabIndexSelected;
	public int TabIndexSelected
	{
		get => _TabIndexSelected;
		set
		{
			if (value == _TabIndexSelected)
				return;

			this.RaisePropertyChanging(nameof(TabIndexSelected));
			this.RaisePropertyChanging(nameof(IsKeysTabSelected));
			this.RaisePropertyChanging(nameof(IsVaultsSelected));
			this.RaisePropertyChanging(nameof(IsActivitySelected));
			this.RaisePropertyChanging(nameof(IsMainListVisible));
			_TabIndexSelected = value;
			this.RaisePropertyChanged(nameof(TabIndexSelected));
			this.RaisePropertyChanged(nameof(IsKeysTabSelected));
			this.RaisePropertyChanged(nameof(IsVaultsSelected));
			this.RaisePropertyChanged(nameof(IsActivitySelected));
			this.RaisePropertyChanged(nameof(IsMainListVisible));

		}
	}
	public bool IsKeysTabSelected => TabIndexSelected is 0;
	public bool IsVaultsSelected => TabIndexSelected is 1;
	public bool IsActivitySelected => TabIndexSelected is 2;
	public bool IsMainListVisible => TabIndexSelected is 0 or 1;

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
		}
	}
	public string SelectedKeyName => _SelectedKey?.Name ?? "Unknown Key";
	public string SelectedKeyDescriptor => _SelectedKey?.Fingerprint ?? "Unknown";

	public async void SyncKeys()
	{
		IsSyncing = true;
		this.RaisePropertyChanging(nameof(Keys));
		this.RaisePropertyChanging(nameof(KeyCount));

		await KeyStore.SyncKeys();

		var obsKeys = KeyStore.PublicKeys
			.Select(x => new ObservableSshKey() { Name = x.Name, Fingerprint = x.Fingerprint });

		Keys.Clear();
		Keys.AddRange(obsKeys);

		this.RaisePropertyChanged(nameof(Keys));
		this.RaisePropertyChanged(nameof(KeyCount));
		IsSyncing = false;
	}
}
