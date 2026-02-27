using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

using Keyden.OnePassword.SDK;
using Keyden.OnePassword.SDK.Types;

namespace Keyden;

// https://developer.1password.com/docs/sdks/desktop-app-integrations
public sealed partial class OnePassSdkSshKeyStore : ISshKeyStore, ISshKeyOptionsStore
{
	[JsonSerializable(typeof(PrivateKeyInfo))]
	internal partial class JsonContext : JsonSerializerContext
	{
	}

	internal record struct PrivateKeyInfo()
	{
		[JsonPropertyName("publicKey")]
		public required string PublicKey { get; set; }

		[JsonPropertyName("fingerprint")]
		public required string Fingerprint { get; set; }

		[JsonPropertyName("keyType")]
		public required string KeyType { get; set; }
	}

	private ISystemServices SystemServices { get; }
	private OnePassSdk Sdk { get; }
	public string AccountName
	{
		get => Sdk.AccountName ?? string.Empty;
		set => Sdk.AccountName = value;
	}

	public OnePassSdkSshKeyStore(ISystemServices systemServices)
	{
		SystemServices = systemServices;
		Sdk = new OnePassSdk(new()
		{
			IntegrationName = "Keyden",
			IntegrationVersion = Verlite.Version.Full,
		});
	}

	private readonly static UTF8Encoding Utf8 = new(encoderShouldEmitUTF8Identifier: false);

	private List<SshKey> PublicKeys { get; } = [];
	private List<SshKey> PrivateKeys { get; } = [];

	IReadOnlyList<SshKey> ISshKeyStore.PublicKeys => PublicKeys;

	ValueTask<SshKey> ISshKeyStore.GetPrivateKey(SshKey publicKey, CancellationToken ct)
	{
		var matchingKey = PrivateKeys
			.Where(k => k.PublicKey.Span.SequenceEqual(publicKey.PublicKey.Span))
			.FirstOrDefault();

		return new(matchingKey);
	}

	async Task ISshKeyStore.SyncKeys(CancellationToken ct)
	{
		var sw = Stopwatch.StartNew();

		var newIds = new List<string>();
		var newKeys = new List<SshKey>();

		foreach (var vault in await Sdk.GetAllVaultsAsync())
		{
			var itemIds = new List<string>();
			foreach (var itemOverview in await Sdk.ListItemsAsync(vault.Id, []))
				if (itemOverview.Category == ItemCategory.SshKey)
					itemIds.Add(itemOverview.Id);

			if (itemIds.Count == 0)
				continue;

			var items = await Sdk.GetItemsAsync(vault.Id, itemIds);

			foreach (var itemResponse in items.IndividualResponses)
			{
				if (itemResponse.Content is null)
					continue;

				var item = itemResponse.Content.Value;
				string? fingerprint = null;
				string? publicKey = null;
				string? privateKey = null;
				
				foreach (var field in item.Fields)
				{
					if (field.FieldType != ItemFieldType.SshKey)
						continue;

					if (field.Id is "public_key")
						publicKey = field.Value;
					else if (field.Id is "private_key")
					{
						var privateKeySecretRef = $"op://{vault.Id}/{item.Id}/{field.Id}?ssh-format=openssh";
						privateKey = await Sdk.ResolveSecretAsync(privateKeySecretRef).WaitAsync(ct);

						if (field.Details.HasValue && field.Details.Value.Type is "SshKey")
						{
							try
							{
								var keyInfo = JsonSerializer.Deserialize(field.Details.Value.Content, JsonContext.Default.PrivateKeyInfo);
								publicKey = keyInfo.PublicKey;
								fingerprint = keyInfo.Fingerprint;
							}
							catch (JsonException)
							{
								break;
							}
						}
					}
				}

				if (publicKey is null || privateKey is null || fingerprint is null)
					continue;

				newIds.Add(item.Id);
				newKeys.Add(new()
				{
					Id = item.Id,
					Name = item.Title,
					Fingerprint = fingerprint,
					PublicKeyText = publicKey,
					PublicKey = Convert.FromBase64String(publicKey[publicKey.IndexOf(' ')..]),
					PrivateKey = Utf8.GetBytes(privateKey),
				});
			}
		}

		PrivateKeys.Clear();
		PublicKeys.Clear();
		PrivateKeys.AddRange(newKeys);
		PublicKeys.AddRange(newKeys.Select(k => k with { PrivateKey = default }));

		Debug.WriteLine($"Synced with op in {sw.Elapsed.TotalSeconds} seconds");
	}

	private readonly HashSet<string> DirtyOptions = [];
	private readonly Dictionary<string, SshKeyOptions> Options = [];
	private const string KeydenOptionsEntryName = "Keyden Options";

	SshKeyOptions? ISshKeyOptionsStore.GetKeyOptions(string id)
	{
		if (Options.TryGetValue(id, out var options))
			return options;
		else
			return null;
	}

	void ISshKeyOptionsStore.SetKeyOptions(string id, SshKeyOptions? options)
	{
		if (options is null)
		{
			if (Options.Remove(id))
				DirtyOptions.Add(id);
		}
		else
		{

			if (Options.TryGetValue(id, out var oldOptions) && options.Value != oldOptions)
				DirtyOptions.Add(id);
			else if (options.Value != new SshKeyOptions())
				DirtyOptions.Add(id);

			Options[id] = options.Value;
		}
	}

	async Task ISshKeyOptionsStore.SyncKeyOptions(CancellationToken ct)
	{
		var jsonSerializerOptions = new JsonSerializerOptions()
		{
			WriteIndented = true,
		};


		var personalVaultId = string.Empty;
		foreach (var vault in await Sdk.GetAllVaultsAsync().WaitAsync(ct))
		{
			if (vault.VaultType == "personal")
			{
				personalVaultId = vault.Id;
				break;
			}
		}
		if (personalVaultId is null)
			throw new BackendException("SyncKeyOptions(): Could not find personal vault");

		Item? optionsItem = null;
		foreach (var item in await Sdk.ListItemsAsync(personalVaultId, []).WaitAsync(ct))
		{
			if (item.Category == ItemCategory.SecureNote && item.Title is KeydenOptionsEntryName)
			{
				optionsItem = await Sdk.GetItemAsync(personalVaultId, item.Id);
				break;
			}
		}

		if (!optionsItem.HasValue)
		{
			optionsItem = Sdk.CreateItem(new()
			{
				Title = KeydenOptionsEntryName,
				VaultId = personalVaultId,
				Category = ItemCategory.SecureNote,
				Fields = [
					new ItemField
						{
							Id = "json",
							Title = "json",
							FieldType = ItemFieldType.Text,
							Value = "{}",
							SectionId = "",
						}
				],
				Sections = [
					new() { Id = "", Title = "" }
				]
			});
		}

		var optionsJson = string.Empty;
		var jsonFieldIndex = -1;

		for (int i = 0; i < optionsItem.Value.Fields.Count; i++)
		{
			var field = optionsItem.Value.Fields[i];

			optionsJson = field.Value;
			jsonFieldIndex = i;
			break;
		}

		if (jsonFieldIndex < 0)
			throw new BackendException("Options \"Keyden Options\" -> \"json\" does not exist, automatic creation not yet supported. Please create manually");

		bool changed = DirtyOptions.Count > 0;
		JsonObject jsonDoc;

		if (string.IsNullOrEmpty(optionsJson))
			jsonDoc = new JsonObject();
		else
			jsonDoc = JsonNode.Parse(optionsJson)?.AsObject()
				?? throw new BackendException($"Failed to parse options json");

		var unprocessedKeys = new HashSet<string>(Options.Keys);
		var toRemoveKeys = new HashSet<string>();

		foreach (var field in jsonDoc)
		{
			if (field.Value?.AsObject() is not JsonObject obj)
				continue;

			var keyId = field.Key;
			var upstreamOptions = KeyOptionsJson.ReadOptionsNode(obj);

			unprocessedKeys.Remove(keyId);

			if (DirtyOptions.Contains(keyId))
			{
				if (Options.TryGetValue(keyId, out var downstreamOptions))
					KeyOptionsJson.WriteOptionsNode(obj, downstreamOptions);
				else
					toRemoveKeys.Add(keyId);
			}
			else
				Options[keyId] = upstreamOptions;
		}

		foreach (var removedKey in toRemoveKeys)
			jsonDoc.Remove(removedKey);
		foreach (var unprocessedKey in unprocessedKeys)
		{
			changed = true;
			var obj = new JsonObject();
			var downstreamOptions = Options[unprocessedKey];
			KeyOptionsJson.WriteOptionsNode(obj, downstreamOptions);
			jsonDoc[unprocessedKey] = obj;
		}

		if (changed)
		{
			var fieldsClone = optionsItem.Value.Fields.ToArray();
			fieldsClone[jsonFieldIndex].Value = jsonDoc.ToJsonString(jsonSerializerOptions);
			optionsItem = optionsItem.Value with { Fields = fieldsClone };

			DirtyOptions.Clear();
			await Sdk.UpdateItemAsync(optionsItem.Value);
		}
	}
}
