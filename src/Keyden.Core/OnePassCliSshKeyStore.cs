using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace Keyden;

public sealed class OnePassCliSshKeyStore : ISshKeyStore, ISshKeyOptionsStore
{
	private readonly static UTF8Encoding Utf8 = new(encoderShouldEmitUTF8Identifier: false);
	private static async Task<string> Op(string args, CancellationToken ct, string? stdIn = null)
	{
		Process proc;
		try
		{
			proc = Process.Start(
				new ProcessStartInfo("op", args)
				{
					UseShellExecute = false,
					CreateNoWindow = true,
					RedirectStandardInput = true,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					StandardInputEncoding = Utf8,
					StandardOutputEncoding = Utf8,
					StandardErrorEncoding = Utf8,
				}) ?? throw new BackendException("Failed to execute `op`, is 1Password CLI installed?");
		}
		catch (Exception ex)
		{
			throw new BackendException($"Failed to execute `op`, is 1Password CLI installed?\n\n{ex.Message}");
		}

		if (stdIn is not null)
		{
			await proc.StandardInput.WriteAsync(stdIn);
			await proc.StandardInput.FlushAsync();
		}
		proc.StandardInput.Close();

		var (stdOut, stdErr) = (proc.StandardOutput.ReadToEndAsync(ct), proc.StandardError.ReadToEndAsync(ct));
		await proc.WaitForExitAsync(ct);

		if (proc.ExitCode != 0)
			throw new BackendException($"Exit code {proc.ExitCode} returned by op:\n{await stdErr}");

		return await stdOut;
	}

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

		var procOutput = await Op("item list --categories \"SSH Key\" --format json", ct);
		var keysDoc = JsonDocument.Parse(procOutput);

		var tasks = new List<Task>();

		var newIds = new List<string>();
		var newKeys = new List<SshKey>();
		foreach (var item in keysDoc.RootElement.EnumerateArray())
		{
			var id = item.GetProperty("id").GetString();
			var name = item.GetProperty("title").GetString();

			if (!item.TryGetProperty("additional_information", out var additionalInfoElm))
				continue;
			var fingerprint = additionalInfoElm.GetString();

			ArgumentException.ThrowIfNullOrEmpty(id);
			ArgumentException.ThrowIfNullOrEmpty(name);
			ArgumentException.ThrowIfNullOrEmpty(fingerprint);

			newIds.Add(id);
			newKeys.Add(new()
			{
				Id = id,
				Name = name,
				Fingerprint = fingerprint,
			});
		}

		var ids = new StringBuilder();
		bool first = true;
		ids.Append('[');
		foreach (var newId in newIds)
		{
			if (!first)
				ids.Append(',');
			else
				first = false;
			ids.Append($$"""{"id":"{{newId}}"}""");
		}
		ids.Append(']');

		var itemGetOutput = await Op("item get --format json --fields label=public_key,label=private_key", ct, ids.ToString());

		// TODO: dotnet 9 will make this Action stuff no longer required
		new Action(() =>
		{
			var outputBytes = Utf8.GetBytes(itemGetOutput);
			var outputSpan = (Span<byte>)outputBytes;
			
			for (int i = 0; i < newKeys.Count; i++)
			{
				// we have to recreate this each time, as 1pass emits "invalid" json
				var reader = new Utf8JsonReader(outputSpan);
				var keyDoc = JsonDocument.ParseValue(ref reader);

				// update the span slice off the previous non-seperated json
				var end = reader.TokenStartIndex + 1; // +1 to consume the end token
				outputSpan = outputSpan.Slice((int)end);

				string? publicKey = null;
				string? privateKey = null;

				foreach (var field in keyDoc.RootElement.EnumerateArray())
				{
					var fieldId = field.GetProperty("id").GetString();

					if (fieldId == "public_key")
						publicKey = field.GetProperty("value").GetString();
					else if (fieldId == "private_key")
					{
						privateKey = field.GetProperty("value").GetString();

						if (
							field.TryGetProperty("ssh_formats", out var sshFormats) &&
							sshFormats.TryGetProperty("openssh", out var openSshKey))
						{
							privateKey = openSshKey.GetProperty("value").GetString();
						}
					}
				}

				if (publicKey is null || privateKey is null)
					return;

				newKeys[i] = newKeys[i] with
				{
					PublicKeyText = publicKey, 
					PublicKey = Convert.FromBase64String(publicKey.Substring(publicKey.IndexOf(' '))),
					PrivateKey = Encoding.ASCII.GetBytes(privateKey),
				};
			}
		})();

		PrivateKeys.Clear();
		PublicKeys.Clear();
		PrivateKeys.AddRange(newKeys);
		PublicKeys.AddRange(newKeys.Select(k => k with { PrivateKey = default }));

		Debug.WriteLine($"Synced with op in {sw.Elapsed.TotalSeconds} seconds");
	}

	private readonly HashSet<string> DirtyOptions = [];
	private readonly Dictionary<string, SshKeyOptions> Options = [];
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

	private const string KewardenOptionsEntryName = "\"Keyden Options\"";

	async Task ISshKeyOptionsStore.SyncKeyOptions(CancellationToken ct)
	{
		var jsonSerializerOptions = new JsonSerializerOptions()
		{
			WriteIndented = true,
		};

		string? opEntryJson;
		try
		{
			opEntryJson = await Op($"item get {KewardenOptionsEntryName} --format json", ct);
		}
		catch (BackendException e) when (e.Message.Contains("isn't an item. Specify the item with its UUID", StringComparison.Ordinal))
		{
			opEntryJson = null;
		}

		if (opEntryJson is null)
		{
			var templateJson =
				$$"""
				{
					"title": {{KewardenOptionsEntryName}},
					"category": "SECURE_NOTE",
					"fields": [
						{
							"id": "notesPlain",
							"type": "STRING",
							"purpose": "NOTES",
							"label": "notesPlain",
							"value": ""
						},
						{
							"type": "STRING",
							"label": "json",
							"value": "{}"
						}
					]
				}
				""";
			opEntryJson = await Op(@"item create --format json --no-color -", ct, stdIn: templateJson);
		}

		var optionsDoc = JsonNode.Parse(opEntryJson)?.AsObject()
			?? throw new BackendException($"Couldn't locale main options object in {KewardenOptionsEntryName}");

		var fields = optionsDoc["fields"]!.AsArray();
		JsonNode? optionsJsonField = null;
		string optionsJson = string.Empty;

		foreach (var field in fields)
		{
			if (field is null)
				continue;
			if (field["label"]?.GetValue<string>() != "json")
				continue;

			var optionsJsonNode = field["value"]?.AsValue()
				?? throw new BackendException("Couldn't parse json: expected value");
			optionsJson = optionsJsonNode.GetValue<string>();
			optionsJsonField = field;
			break;
		}

		if (optionsJsonField is null)
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
			optionsJsonField["value"] = JsonValue.Create(jsonDoc.ToJsonString(jsonSerializerOptions));
			var fullJson = optionsDoc.ToJsonString();

			DirtyOptions.Clear();
			await Op($"item edit {KewardenOptionsEntryName} --format json --no-color", ct, fullJson);
		}
	}
}
