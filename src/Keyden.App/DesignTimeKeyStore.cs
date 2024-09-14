using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;
using System.Security.Cryptography;

namespace Keyden;

internal class DesignTimeKeyStore : ISshKeyStore, ISshKeyOptionsStore
{
	private static readonly string[] RandomWords = ["Test", "ECC", "RSA", "Key", "Home", "Work"];
	private static ReadOnlyMemory<byte> RandomPublicKey()
	{
		Memory<byte> randomBytes = new byte[64];
		RandomNumberGenerator.Fill(randomBytes.Span);
		return randomBytes;
	}
	private static SshKey GenerateRandomKey()
	{
		var words = Enumerable.Range(0, RandomNumberGenerator.GetInt32(2, 4))
			.Select(_ => RandomNumberGenerator.GetInt32(0, RandomWords.Length))
			.Select(x => RandomWords[x]);

		var publicKey = RandomPublicKey();
		var pubKeyForHumans = $"ssh-ed25519 {Convert.ToBase64String(publicKey.Span)}";
		var id = RandomNumberGenerator.GetHexString(256 / 8);

		return new()
		{
			Id = id,
			Name = string.Join(" ", words),
			Fingerprint = $"SHA256:{id}",
			PublicKey = publicKey,
			PublicKeyText = pubKeyForHumans,
			PrivateKey =
				"""
				-----BEGIN OPENSSH PRIVATE KEY-----
				b3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAAAAAAAAABAAAAMwAAAAtzc2gtZW
				QyNTUxOQAAACB5Wj0HBGqt5wPsz0VtmqD9HeXh8ep651CZyzTD7iJbFwAAAJDthtsL7Ybb
				CwAAAAtzc2gtZWQyNTUxOQAAACB5Wj0HBGqt5wPsz0VtmqD9HeXh8ep651CZyzTD7iJbFw
				AAAEA6OG/DKrT98jac6G6cPagI6PH7jpTc5P+Bs/V26BDVKXlaPQcEaq3nA+zPRW2aoP0d
				5eHx6nrnUJnLNMPuIlsXAAAAC2theWxhQExvZ29zAQI=
				-----END OPENSSH PRIVATE KEY-----
				"""u8.ToArray(),
		};
	}

	public DesignTimeKeyStore()
	{
		PrivateKeys = new()
			{
				GenerateRandomKey(),
				GenerateRandomKey(),
				GenerateRandomKey(),
				GenerateRandomKey(),
				GenerateRandomKey(),
				GenerateRandomKey(),
				GenerateRandomKey(),
				GenerateRandomKey(),
				GenerateRandomKey(),
				GenerateRandomKey(),
				GenerateRandomKey(),
				GenerateRandomKey(),
				GenerateRandomKey(),
				GenerateRandomKey(),
				GenerateRandomKey(),
				GenerateRandomKey(),
				GenerateRandomKey(),
				GenerateRandomKey(),
				GenerateRandomKey(),
				GenerateRandomKey(),
				GenerateRandomKey(),
				GenerateRandomKey(),
			};

		PublicKeys = PrivateKeys
			.Select(x => x with { PrivateKey = default })
			.ToList();
	}

	private List<SshKey> PrivateKeys { get; }
	public IReadOnlyList<SshKey> PublicKeys { get; }

	ValueTask<SshKey> ISshKeyStore.GetPrivateKey(SshKey publicKey, CancellationToken ct)
	{
		var matchingKey = PrivateKeys
			.Where(k => k.PublicKey.Span.SequenceEqual(publicKey.PublicKey.Span))
			.FirstOrDefault();

		return new(matchingKey);
	}

	async Task ISshKeyStore.SyncKeys(CancellationToken ct)
	{
		await Task.Delay(500);
	}

	private Dictionary<string, SshKeyOptions> KeyOptions { get; } = new();

	SshKeyOptions? ISshKeyOptionsStore.GetKeyOptions(string id)
	{
		if (KeyOptions.TryGetValue(id, out var keyOptions))
			return keyOptions;
		else
			return null;
	}

	void ISshKeyOptionsStore.SetKeyOptions(string id, SshKeyOptions? options)
	{
		if (options is null)
			KeyOptions.Remove(id);
		else
			KeyOptions[id] = options.Value;
	}

	async Task ISshKeyOptionsStore.SyncKeyOptions(CancellationToken ct)
	{
		await Task.Delay(500, ct);
	}
}
