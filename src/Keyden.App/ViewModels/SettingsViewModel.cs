using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;

using Microsoft.Win32;

using ReactiveUI;

namespace Keyden.ViewModels;

public class SettingsViewModel : ViewModelBase
{
	private KeyStoreController? KeyStoreController { get; } = null;
	private DeveloperTestKeyStore? DevTestKeyStore { get; } = null;
	private OnePassCliSshKeyStore? OnePassCliSshKeyStore { get; } = null;

	public KeydenSettings Settings { get; }
	private const string SettingsLocation = "settings.json";

	public SettingsViewModel(KeydenSettings settings)
	{
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
			this.RaisePropertyChanging(nameof(IsAdvancedTabSelected));
			_TabIndexSelected = value;
			this.RaisePropertyChanged(nameof(TabIndexSelected));
			this.RaisePropertyChanged(nameof(IsGeneralTabSelected));
			this.RaisePropertyChanged(nameof(IsAdvancedTabSelected));
		}
	}
	public bool IsGeneralTabSelected => TabIndexSelected == 0;
	public bool IsAdvancedTabSelected => TabIndexSelected == 1;

	public IReadOnlyList<KeystoreBackend> KeystoreBackends =>
		Settings.DeveloperMode
			? [KeystoreBackend.None, KeystoreBackend.OnePassCLI, KeystoreBackend.DeveloperTest]
			: [KeystoreBackend.None, KeystoreBackend.OnePassCLI];

	public virtual bool AutomaticallyStartup
	{
		get
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				using RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", false)!;

				var path = Environment.ProcessPath;
				if (string.IsNullOrEmpty(path))
					return false;

				return key.GetValue("Keyden") as string == $"\"{path}\" --hide";
			}
			return false;
		}
		set
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				using RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true)!;

				var path = Environment.ProcessPath;
				if (string.IsNullOrEmpty(path))
					return;

				this.RaisePropertyChanging(nameof(AutomaticallyStartup));
				if (value)
					key.SetValue("Keyden", $"\"{path}\" --hide");
				else
					key.DeleteValue("Keyden");
				this.RaisePropertyChanged(nameof(AutomaticallyStartup));
			}
		}
	}
}

public class DesignSettingsViewModel : SettingsViewModel
{
	public DesignSettingsViewModel() : base(new())
	{
	}

	public override bool AutomaticallyStartup { get; set; }
}
