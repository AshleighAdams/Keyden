using CommunityToolkit.Mvvm.ComponentModel;

using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace Keyden;

public partial class KeydenSettings : ObservableObject
{
	private KeystoreBackend _KeystoreBackend;
	public KeystoreBackend KeystoreBackend
	{
		get => _KeystoreBackend;
		set => SetProperty(ref _KeystoreBackend, value);
	}

	private float _AuthButtonEnableDelay = 0.5f;
	public float AuthButtonEnableDelay
	{
		get => _AuthButtonEnableDelay;
		set => SetProperty(ref _AuthButtonEnableDelay, value);
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

	private AuthenticationMode _AuthenticationMode = AuthenticationMode.System;
	public AuthenticationMode AuthenticationMode
	{
		get => _AuthenticationMode;
		set => SetProperty(ref _AuthenticationMode, value);
	}

	public string Salt { get; set; } = RandomNumberGenerator.GetString("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 32);
	public string PinHash { get; set; } = string.Empty;

	[JsonIgnore]
	public string AuthenticationPin
	{
		get => "hello there :)";
		set
		{
			var sb = new StringBuilder();

			var result = SHA256.HashData(Encoding.UTF8.GetBytes(Salt + value));
			foreach (byte b in result)
				sb.Append(b.ToString("x2"));

			OnPropertyChanging(nameof(AuthenticationPin));
			PinHash = sb.ToString();
			OnPropertyChanged(nameof(AuthenticationPin));
		}
	}

	public bool CheckPin(string pin)
	{
		var sb = new StringBuilder();

		var result = SHA256.HashData(Encoding.UTF8.GetBytes(Salt + pin));
		foreach (byte b in result)
			sb.Append(b.ToString("x2"));

		return PinHash == sb.ToString();
	}
}

public enum KeystoreBackend
{
	None,
	DeveloperTest,
	OnePassCLI,
}

public enum AuthenticationMode
{
	System,
	InternalPIN,
}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(KeydenSettings))]
internal partial class SettingsGenerationContext : JsonSerializerContext
{
}
