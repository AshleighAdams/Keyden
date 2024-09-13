using Avalonia;
using Keyden.Views;

using Microsoft.Win32;

using ReactiveUI;

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Keyden.ViewModels;

internal class KeydenAppSettings
{
	public KeystoreBackend KeystoreBackend { get; set; }
	public bool DeveloperMode { get; set; }
}

public enum KeystoreBackend
{
	None,
	DeveloperTest,
	OnePassCLI,
}

public class SettingsViewModel : ViewModelBase
{
	private KeyStoreController? KeyStoreController { get; } = null;
	private DeveloperTestKeyStore? DevTestKeyStore { get; } = null;
	private OnePassCliSshKeyStore? OnePassCliSshKeyStore { get; } = null;
	private IFileSystem FileSystem { get; }

	private KeydenAppSettings Settings { get; }
	private const string SettingsLocation = "settings.json";

	public SettingsViewModel() : this(false)
	{
		KeyStoreController = App.GetService<KeyStoreController>();
		DevTestKeyStore = App.GetKeyedService<DeveloperTestKeyStore>("devtest");
		OnePassCliSshKeyStore = App.GetKeyedService<OnePassCliSshKeyStore>("op");

		ApplyAllSettings();
	}

	public SettingsViewModel(bool designMode)
	{
		FileSystem = App.GetService<IFileSystem>();

		if (FileSystem.TryReadBytes(SettingsLocation, out var contents))
		{
			try
			{
				Settings = JsonSerializer.Deserialize(
					contents.Span,
					SourceGenerationContext.Default.KeydenAppSettings) ?? new();
			}
			catch
			{
				Settings = new();
			}
		}
		else
			Settings = new();
	}

	public void ApplyAllSettings()
	{
		ApplyKeystoreBackend();
	}

	public void SaveSettings()
	{
		var jsonString = JsonSerializer.Serialize(
			Settings,
			SourceGenerationContext.Default.KeydenAppSettings);
		FileSystem.TryWriteBytes(SettingsLocation, Encoding.UTF8.GetBytes(jsonString));
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
			_TabIndexSelected = value;
			this.RaisePropertyChanged(nameof(TabIndexSelected));
			this.RaisePropertyChanged(nameof(IsGeneralTabSelected));
		}
	}
	public bool IsGeneralTabSelected => TabIndexSelected == 0;
	public bool IsAdvancedTabSelected => TabIndexSelected == 1;

	public KeystoreBackend KeystoreBackend
	{
		get => Settings.KeystoreBackend;
		set
		{
			if (value == Settings.KeystoreBackend)
				return;

			this.RaisePropertyChanging(nameof(KeystoreBackend));
			Settings.KeystoreBackend = value;
			this.RaisePropertyChanged(nameof(KeystoreBackend));

			ApplyKeystoreBackend();
			SaveSettings();
		}
	}
	public IReadOnlyList<KeystoreBackend> KeystoreBackends { get; } = [ KeystoreBackend.None, KeystoreBackend.DeveloperTest, KeystoreBackend.OnePassCLI ];

	public void ApplyKeystoreBackend()
	{
		if (KeyStoreController is null)
			return;

		switch (Settings.KeystoreBackend)
		{
			case KeystoreBackend.OnePassCLI:
				KeyStoreController.BaseKeyStore = OnePassCliSshKeyStore;
				KeyStoreController.BaseOptionsStore = OnePassCliSshKeyStore;
				break;
			case KeystoreBackend.DeveloperTest:
				KeyStoreController.BaseKeyStore = DevTestKeyStore;
				KeyStoreController.BaseOptionsStore = DevTestKeyStore;
				break;
			case KeystoreBackend.None:
			default:
				KeyStoreController.BaseKeyStore = null;
				KeyStoreController.BaseOptionsStore = null;
				break;
		}
	}

	public bool AutomaticallyStartup
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
	public DesignSettingsViewModel() : base(true)
	{
		ApplyAllSettings();
	}
}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(KeydenAppSettings))]
internal partial class SourceGenerationContext : JsonSerializerContext
{
}
