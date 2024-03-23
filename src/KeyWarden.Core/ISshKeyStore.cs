using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace KeyWarden;

public interface ISshKeyStore
{
	Task SyncKeys(CancellationToken ct = default);
	IReadOnlyList<SshKey> PublicKeys { get; }
	ValueTask<SshKey> GetPrivateKey(SshKey publicKey, CancellationToken ct);
}

public sealed class OnePassCliSshKeyStore : ISshKeyStore
{
	private List<SshKey> PublicKeys { get; } = new();
	private List<SshKey> PrivateKeys { get; } = new();
	IReadOnlyList<SshKey> ISshKeyStore.PublicKeys => PublicKeys;

	public ValueTask<SshKey> GetPrivateKey(SshKey publicKey, CancellationToken ct)
	{
		var matchingKey = PrivateKeys
			.Where(k => k.PublicKey.Span.SequenceEqual(publicKey.PublicKey.Span))
			.FirstOrDefault();

		return new(matchingKey);
	}

	private async Task<string> Run(string cmd)
	{
		var info = new ProcessStartInfo("op", cmd)
		{
			UseShellExecute = false,
			WindowStyle = ProcessWindowStyle.Hidden,
			CreateNoWindow = true,
			RedirectStandardOutput = true,
		};

		var proc = Process.Start(info) ?? throw new SystemException("Failed to start op");
		var output = await proc!.StandardOutput.ReadToEndAsync();
		await proc.WaitForExitAsync();

		return output;
	}

	public async Task SyncKeys(CancellationToken ct = default)
	{
		var keysJson = await Run("item list --categories \"SSH Key\" --format json");
		var keysDoc = JsonDocument.Parse(keysJson);

		var newKeys = new List<SshKey>();
		foreach (var item in keysDoc.RootElement.EnumerateArray())
		{
			var id = item.GetProperty("id").GetString();
			var name = item.GetProperty("title").GetString();
			var fingerprint = item.GetProperty("additional_information").GetString();

			ArgumentException.ThrowIfNullOrEmpty(id);
			ArgumentException.ThrowIfNullOrEmpty(name);
			ArgumentException.ThrowIfNullOrEmpty(fingerprint);

			var keyJson = await Run($"item get {id} --format json");
			var keyDoc = JsonDocument.Parse(keyJson);

			string? publicKey = null;
			string? privateKey = null;

			foreach (var field in keyDoc.RootElement.GetProperty("fields").EnumerateArray())
			{
				var fieldId = field.GetProperty("id").GetString();

				if (fieldId == "public_key")
					publicKey = field.GetProperty("value").GetString();
				else if (fieldId == "private_key")
					privateKey = field.GetProperty("value").GetString();
			}

			if (publicKey is null || privateKey is null)
				continue;

			var publicKeyBytes = Convert.FromBase64String(publicKey.Substring(publicKey.IndexOf(' ')));

			newKeys.Add(new() { Name = name, Fingerprint = fingerprint, PublicKey = publicKeyBytes, PrivateKey = Encoding.ASCII.GetBytes(privateKey) });
		}

		PrivateKeys.Clear();
		PublicKeys.Clear();
		PrivateKeys.AddRange(newKeys);
		PublicKeys.AddRange(newKeys.Select(k => k with { PrivateKey = default }));
	}
}
