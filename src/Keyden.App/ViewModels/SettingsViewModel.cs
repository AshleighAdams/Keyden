using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Microsoft.Win32;

using ReactiveUI;

namespace Keyden.ViewModels;

public class SettingsViewModel : ViewModelBase
{
	private ISystemServices SystemServices { get; }
	public KeydenSettings Settings { get; }

	public SettingsViewModel(
		ISystemServices systemServices,
		KeydenSettings settings)
	{
		SystemServices = systemServices;
		Settings = settings;
	}

	private int _TabIndexSelected = 0;
	public int TabIndexSelected
	{
		get => _TabIndexSelected;
		set
		{
			if (value == _TabIndexSelected)
				return;

			this.RaisePropertyChanging(nameof(TabIndexSelected));
			this.RaisePropertyChanging(nameof(IsGeneralTabSelected));
			this.RaisePropertyChanging(nameof(IsSecurityTabSelected));
			this.RaisePropertyChanging(nameof(IsAdvancedTabSelected));
			_TabIndexSelected = value;
			this.RaisePropertyChanged(nameof(TabIndexSelected));
			this.RaisePropertyChanged(nameof(IsGeneralTabSelected));
			this.RaisePropertyChanged(nameof(IsSecurityTabSelected));
			this.RaisePropertyChanged(nameof(IsAdvancedTabSelected));
		}
	}
	public bool IsGeneralTabSelected => TabIndexSelected == 0;
	public bool IsSecurityTabSelected => TabIndexSelected == 2;
	public bool IsAdvancedTabSelected => TabIndexSelected == 1;

	public IReadOnlyList<KeystoreBackend> KeystoreBackends =>
		Settings.DeveloperMode
			? [KeystoreBackend.None, KeystoreBackend.OnePassCLI, KeystoreBackend.DeveloperTest]
			: [KeystoreBackend.None, KeystoreBackend.OnePassCLI];

	public IReadOnlyList<AuthenticationMode> AuthenticationModes { get; } = [AuthenticationMode.System, AuthenticationMode.InternalPIN];

	public bool AutomaticallyStartup
	{

		get => SystemServices.AutomaticallyStartApp;
		set
		{
			this.RaisePropertyChanging(nameof(AutomaticallyStartup));
			SystemServices.AutomaticallyStartApp = value;
			this.RaisePropertyChanged(nameof(AutomaticallyStartup));
		}
	}
}

public class DesignSettingsViewModel : SettingsViewModel
{
	public DesignSettingsViewModel() : base(new GenericSystemServices(), new())
	{
	}
}
