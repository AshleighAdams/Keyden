using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace KeyWarden;

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

	private async Task<string> Run(string cmd, CancellationToken ct)
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
		await proc.WaitForExitAsync(ct);

		return output;
	}

	public async Task SyncKeys(CancellationToken ct = default)
	{
		var sw = Stopwatch.StartNew();

		var proc = Process.Start(
			new ProcessStartInfo("op", "item list --categories \"SSH Key\" --format json")
			{
				UseShellExecute = false,
				WindowStyle = ProcessWindowStyle.Hidden,
				CreateNoWindow = true,
				RedirectStandardOutput = true,
			}) ?? throw new SystemException("Failed to start op");

		var keysDoc = await JsonDocument.ParseAsync(proc.StandardOutput.BaseStream, cancellationToken: ct);
		//await proc.WaitForExitAsync(ct);

		var tasks = new List<Task>();

		var newIds = new List<string>();
		var newKeys = new List<SshKey>();
		foreach (var item in keysDoc.RootElement.EnumerateArray())
		{
			var id = item.GetProperty("id").GetString();
			var name = item.GetProperty("title").GetString();
			var fingerprint = item.GetProperty("additional_information").GetString();

			ArgumentException.ThrowIfNullOrEmpty(id);
			ArgumentException.ThrowIfNullOrEmpty(name);
			ArgumentException.ThrowIfNullOrEmpty(fingerprint);

			newIds.Add(id);
			newKeys.Add(new()
			{
				Name = name,
				Fingerprint = fingerprint,
			});
		}

		proc = Process.Start(
			new ProcessStartInfo("op", "item get --format json --fields label=public_key,label=private_key")
			{
				UseShellExecute = false,
				WindowStyle = ProcessWindowStyle.Hidden,
				CreateNoWindow = true,
				RedirectStandardOutput = true,
				RedirectStandardInput = true,
			}) ?? throw new SystemException("Failed to start op");

		var input = proc.StandardInput;
		var output = proc.StandardOutput.BaseStream;

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
		await input.WriteAsync(ids);
		input.Close();

		var ms = new MemoryStream();
		await output.CopyToAsync(ms);

		new Action(() =>
		{
			var outputBytes = ms.ToArray();
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

						if (field.TryGetProperty("ssh_formats", out var sshFormats) &&
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
}
