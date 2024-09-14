using Keyden.ViewModels;

using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Keyden;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(KeyStoreControllerData))]
internal partial class KeyStoreGenerationContext : JsonSerializerContext
{
}

internal class KeyStoreControllerData
{
	public List<SshKey> CachedPublicKeys { get; init; } = [];
	public Dictionary<string, SshKeyOptions> CachedKeyOptions { get; init; } = new();
}

public sealed class KeyStoreController : ISshKeyStore, ISshKeyOptionsStore
{
	public ISshKeyStore? BaseKeyStore { get; set; }
	public ISshKeyOptionsStore? BaseOptionsStore { get; set; }

	private bool HasSynced { get; set; }
	private KeydenSettings Settings { get; }
	private IFileSystem FileSystem { get; }

	private readonly DeveloperTestKeyStore DevTestStore;
	private readonly OnePassCliSshKeyStore OnePassCliStore;

	private KeyStoreControllerData Data { get; }
	private const string DataLocation = "keystore-controller-data.json";

	public KeyStoreController(
		KeydenSettings settings,
		IFileSystem fileSystem)
	{
		Settings = settings;
		FileSystem = fileSystem;

		DevTestStore = App.GetKeyedService<DeveloperTestKeyStore>("devtest");
		OnePassCliStore = App.GetKeyedService<OnePassCliSshKeyStore>("op");

		if (FileSystem.TryReadBytes(DataLocation, out var contents))
		{
			try
			{
				Data = JsonSerializer.Deserialize(
					contents.Span,
					KeyStoreGenerationContext.Default.KeyStoreControllerData) ?? new();
			}
			catch
			{
				Data = new();
			}
		}
		else
			Data = new();

		UpdateBackend();
		Settings.PropertyChanged += Settings_PropertyChanged;
	}

	private void UpdateBackend()
	{
		switch (Settings.KeystoreBackend)
		{
			case KeystoreBackend.OnePassCLI:
				BaseKeyStore = OnePassCliStore;
				BaseOptionsStore = OnePassCliStore;
				break;
			case KeystoreBackend.DeveloperTest:
				BaseKeyStore = DevTestStore;
				BaseOptionsStore = DevTestStore;
				break;
			case KeystoreBackend.None:
			default:
				BaseKeyStore = null;
				BaseOptionsStore = null;
				break;
		}
	}

	private void Settings_PropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(Settings.KeystoreBackend))
			UpdateBackend();
	}

	public IReadOnlyList<SshKey> PublicKeys =>
		HasSynced && BaseKeyStore is not null
			? BaseKeyStore.PublicKeys
			: Data.CachedPublicKeys;

	public SshKeyOptions? GetKeyOptions(string id)
	{
		if (!HasSynced)
		{
			if (!Data.CachedKeyOptions.TryGetValue(id, out var options))
				return null;
			return options;
		}

		return BaseOptionsStore?.GetKeyOptions(id);
	}

	public async ValueTask<SshKey> GetPrivateKey(SshKey publicKey, CancellationToken ct)
	{
		if (BaseKeyStore is null)
			return default;

		if (!HasSynced)
		{
			await SyncKeys(ct);
			await SyncKeyOptions(ct);
		}

		return await BaseKeyStore.GetPrivateKey(publicKey, ct);
	}

	public void SetKeyOptions(string id, SshKeyOptions? options)
	{
		if (options is null)
			Data.CachedKeyOptions.Remove(id);
		else
			Data.CachedKeyOptions[id] = options.Value;

		BaseOptionsStore?.SetKeyOptions(id, options);
	}

	public async Task SyncKeyOptions(CancellationToken ct)
	{
		if (BaseOptionsStore is null)
			throw new BackendException(
				"""
				No backend has been selected.

				Open settings, and select a backend, then try again.
				""");

		await BaseOptionsStore.SyncKeyOptions(ct);

		Data.CachedKeyOptions.Clear();
		foreach (var key in Data.CachedPublicKeys)
		{
			var opts = BaseOptionsStore.GetKeyOptions(key.Id);
			if (opts is null)
				continue;

			Data.CachedKeyOptions[key.Id] = opts.Value;
		}

		var jsonString = JsonSerializer.Serialize(
			Data,
			KeyStoreGenerationContext.Default.KeyStoreControllerData);
		await FileSystem.TryWriteBytesAsync(DataLocation, Encoding.UTF8.GetBytes(jsonString));
	}

	public async Task SyncKeys(CancellationToken ct)
	{
		if (BaseKeyStore is null || BaseOptionsStore is null)
			throw new BackendException(
				"""
				No backend has been selected.

				Open settings, and select a backend, then try again.
				""");

		await BaseKeyStore.SyncKeys(ct);

		Data.CachedPublicKeys.Clear();
		Data.CachedPublicKeys.AddRange(BaseKeyStore.PublicKeys);

		var jsonString = JsonSerializer.Serialize(
			Data,
			KeyStoreGenerationContext.Default.KeyStoreControllerData);
		await FileSystem.TryWriteBytesAsync(DataLocation, Encoding.UTF8.GetBytes(jsonString));

		HasSynced = true;
	}
}
