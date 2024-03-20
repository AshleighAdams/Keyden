using CommunityToolkit.Mvvm.ComponentModel;

using ReactiveUI;

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.Json;

namespace KeyWarden.ViewModels;

public partial class SshKey : ObservableObject
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

public abstract class MainViewModel : ViewModelBase
{
	public string Greeting => "Welcome to Avalonia!";
	public ObservableCollection<SshKey> Keys { get; } = new();
	public string KeyCount => $"{Keys.Count} Keys";

	private SshKey? _SelectedKey;
	public SshKey? SelectedKey
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

	public bool IsKeySelected => _SelectedKey is not null;
	private bool _IsSyncing;
	public bool IsSyncing
	{
		get => _IsSyncing;
		set => this.RaiseAndSetIfChanged(ref _IsSyncing, value, nameof(IsSyncing));
	}
	public string SelectedKeyName => _SelectedKey?.Name ?? "Unknown Key";
	public string SelectedKeyDescriptor => _SelectedKey?.Fingerprint ?? "Unknown";

	public abstract void SyncKeys();

	protected void ReadKeysFromJson(string json)
	{
		var jsonDoc = JsonDocument.Parse(json);

		this.RaisePropertyChanging(nameof(Keys));
		this.RaisePropertyChanging(nameof(KeyCount));
		Keys.Clear();
		foreach (var item in jsonDoc.RootElement.EnumerateArray())
		{
			var name = item.GetProperty("title").GetString();
			var descriptor = item.GetProperty("additional_information").GetString();

			ArgumentException.ThrowIfNullOrEmpty(name);
			ArgumentException.ThrowIfNullOrEmpty(descriptor);

			Keys.Add(new() { Name = name, Fingerprint = descriptor });
		}
		this.RaisePropertyChanged(nameof(KeyCount));
	}
}

public class RuntimeMainViewModel : MainViewModel
{
	public override async void SyncKeys()
	{
		if (IsSyncing)
			return;

		IsSyncing = true;
		try
		{
			var info = new ProcessStartInfo("op", "item list --categories \"SSH Key\" --format json")
			{
				UseShellExecute = false,
				WindowStyle = ProcessWindowStyle.Hidden,
				CreateNoWindow = true,
				RedirectStandardOutput = true,
			};

			var proc = Process.Start(info) ?? throw new SystemException("Failed to start op");
			var json = await proc!.StandardOutput.ReadToEndAsync();
			await proc.WaitForExitAsync();

			ReadKeysFromJson(json);
		}
		finally
		{
			IsSyncing = false;
		}
	}
}

public class DesignMainViewModel : MainViewModel
{
	public DesignMainViewModel()
	{
		Keys.Add(new() { Name = "Main Transport ECC", Fingerprint = "SHA256:2d0b26a4c432b18bdd3297086c29676ccf99298eb66b7111a7c75c22619f418b" });
		Keys.Add(new() { Name = "Main Signing ECC", Fingerprint = "SHA256:729acfbfd6f741ae2eda9f8c01d09a41a344f4259c7910de0acf2c54b2acf709" });
		Keys.Add(new() { Name = "Work Transport ECC", Fingerprint = "SHA256:4519940f07cb26276944c9806a5fb0f20f21e76fa8f734e137c5cd0c545ffb22" });
		Keys.Add(new() { Name = "Work Signing ECC", Fingerprint = "SHA256:8ff11f942a63a29bc23baeb66bf88002fc1b7fad074788611e9f48d4b8576b6d" });
		Keys.Add(new() { Name = "Legacy RSA", Fingerprint = "SHA256:730198f08765bc30755032f68cfd8bd661ee693a21505bcf7d661af9bed9bfd9" });
	}

	public override void SyncKeys()
	{
	}
}
