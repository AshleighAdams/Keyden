using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Keyden;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(KeyStoreControllerData))]
internal partial class SourceGenerationContext : JsonSerializerContext
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
	private IFileSystem FileSystem { get; }

	private KeyStoreControllerData Data { get; }
	private const string DataLocation = "keystore-controller-data.json";

	public KeyStoreController(IFileSystem fileSystem)
	{
		FileSystem = fileSystem;

		if (FileSystem.TryReadBytes(DataLocation, out var contents))
		{
			try
			{
				Data = JsonSerializer.Deserialize(
					contents.Span,
					SourceGenerationContext.Default.KeyStoreControllerData) ?? new();
			}
			catch
			{
				Data = new();
			}
		}
		else
			Data = new();
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
			await SyncKeys(ct);

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
			return;

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
			SourceGenerationContext.Default.KeyStoreControllerData);
		await FileSystem.TryWriteBytesAsync(DataLocation, Encoding.UTF8.GetBytes(jsonString));

	}

	public async Task SyncKeys(CancellationToken ct)
	{
		if (BaseKeyStore is null)
			return;

		await BaseKeyStore.SyncKeys(ct);

		Data.CachedPublicKeys.Clear();
		Data.CachedPublicKeys.AddRange(BaseKeyStore.PublicKeys);

		var jsonString = JsonSerializer.Serialize(
			Data,
			SourceGenerationContext.Default.KeyStoreControllerData);
		await FileSystem.TryWriteBytesAsync(DataLocation, Encoding.UTF8.GetBytes(jsonString));

		HasSynced = true;
	}
}
