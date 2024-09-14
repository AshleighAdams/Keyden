using CommunityToolkit.Mvvm.ComponentModel;

using System.Text.Json.Serialization;

namespace Keyden;

public partial class KeydenSettings : ObservableObject
{
	private KeystoreBackend _KeystoreBackend;
	public KeystoreBackend KeystoreBackend
	{
		get => _KeystoreBackend;
		set => SetProperty(ref _KeystoreBackend, value);
	}

	private bool _DeveloperMode;
	public bool DeveloperMode
	{
		get => _DeveloperMode;
		set
		{
			if (value == _DeveloperMode)
				return;

			OnPropertyChanging(nameof(DeveloperMode));

			_DeveloperMode = value;
			if (!value)
			{
				if (KeystoreBackend == KeystoreBackend.DeveloperTest)
					KeystoreBackend = KeystoreBackend.None;
			}

			OnPropertyChanged(nameof(DeveloperMode));
		}
	}
}

public enum KeystoreBackend
{
	None,
	DeveloperTest,
	OnePassCLI,
}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(KeydenSettings))]
internal partial class SettingsGenerationContext : JsonSerializerContext
{
}
