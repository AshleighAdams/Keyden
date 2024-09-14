using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Keyden;

public sealed class DeveloperTestKeyStore : ISshKeyStore, ISshKeyOptionsStore
{
	private static SshKey CreateKey(string name, string id, string fingerprint, string publicKey, string privateKey)
	{
		return new()
		{
			Id = id,
			Name = name,
			Fingerprint = fingerprint,
			PublicKeyText = publicKey,
			PublicKey = Convert.FromBase64String(publicKey[publicKey.IndexOf(' ')..]),
			PrivateKey = Encoding.ASCII.GetBytes(privateKey),
		};
	}

	public static SshKey[] InitialKeys =
	[
		CreateKey(
			name: "Tag signing key",
			id: "qhhvy45adllqzjal5w3luvl7lm",
			fingerprint: "SHA256:Nn+tt7W4kQeNe+lURxQEoj0xz1Di5MJU1S4F/a6L3nc",
			publicKey: "ssh-ed25519 AAAAC3NzaC1lZDI1NTE5AAAAICFFP46FA/Pk0efFQSxfhwMeU3laqWs59NhLv+BqgjUg",
			privateKey:
				"""
				-----BEGIN OPENSSH PRIVATE KEY-----
				b3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAAAAAAAAABAAAAMwAAAAtzc2gtZW
				QyNTUxOQAAACAhRT+OhQPz5NHnxUEsX4cDHlN5WqlrOfTYS7/gaoI1IAAAAIgcg7obHIO6
				GwAAAAtzc2gtZWQyNTUxOQAAACAhRT+OhQPz5NHnxUEsX4cDHlN5WqlrOfTYS7/gaoI1IA
				AAAECC2WNM5t+bYpnqd1bzxlVrzmCUOWCic1LJAnk7MZK7aiFFP46FA/Pk0efFQSxfhwMe
				U3laqWs59NhLv+BqgjUgAAAAAAECAwQF
				-----END OPENSSH PRIVATE KEY-----
				"""),
		CreateKey(
			name: "Commit signing key",
			id: "u5utdrdxlqu5qv556ewil425xi",
			fingerprint: "SHA256:ADDiWaHNSsUy7QaG82m6PMNQcmstS38LwuNMM4n9ZBQ",
			publicKey: "ssh-ed25519 AAAAC3NzaC1lZDI1NTE5AAAAIPECHV7YlDaqAEcbh+6TKaUE27RrylwlGjR5AwoAkBcW",
			privateKey:
				"""
				-----BEGIN OPENSSH PRIVATE KEY-----
				b3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAAAAAAAAABAAAAMwAAAAtzc2gtZW
				QyNTUxOQAAACDxAh1e2JQ2qgBHG4fukymlBNu0a8pcJRo0eQMKAJAXFgAAAIjdr5JB3a+S
				QQAAAAtzc2gtZWQyNTUxOQAAACDxAh1e2JQ2qgBHG4fukymlBNu0a8pcJRo0eQMKAJAXFg
				AAAEAmh0tMA/d96+JykA98iQKyHwZiyVhsO9TQAqLOdQ9E/vECHV7YlDaqAEcbh+6TKaUE
				27RrylwlGjR5AwoAkBcWAAAAAAECAwQF
				-----END OPENSSH PRIVATE KEY-----
				"""),
		CreateKey(
			name: "Server key",
			id: "kqb42mkf5c3maeprgkg6clb3jm",
			fingerprint: "SHA256:gb4EadCJFaW8i3JnF8xzNBXch7pwmCmMCyYZ1QNawYs",
			publicKey: "ssh-ed25519 AAAAC3NzaC1lZDI1NTE5AAAAIAk61r+4noRxxdckzMHXpohURo+GRUjjKHU3Ja2VwbXH",
			privateKey:
				"""
				-----BEGIN OPENSSH PRIVATE KEY-----
				b3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAAAAAAAAABAAAAMwAAAAtzc2gtZW
				QyNTUxOQAAACAJOta/uJ6EccXXJMzB16aIVEaPhkVI4yh1NyWtlcG1xwAAAIjyhyhn8oco
				ZwAAAAtzc2gtZWQyNTUxOQAAACAJOta/uJ6EccXXJMzB16aIVEaPhkVI4yh1NyWtlcG1xw
				AAAECN4lnLTiMxT0DxpOQFZKAImSi20SngB4gInMUHbRY+Fwk61r+4noRxxdckzMHXpohU
				Ro+GRUjjKHU3Ja2VwbXHAAAAAAECAwQF
				-----END OPENSSH PRIVATE KEY-----
				"""),
		CreateKey(
			name: "Legacy key",
			id: "rgatdoo4zobwyhtsivnod636hu",
			fingerprint: "SHA256:GT4f6j3VaXbIHDWHE6aDpkeR8K8qP5P9MTC/XSQvRXY",
			publicKey: "ssh-rsa AAAAB3NzaC1yc2EAAAADAQABAAACAQDNJ2V8surUV4JAFTB2K4yxnq95pWMXZLWvhfhlCKEeDnTrl3Q3m8zQSqNJVZSqgiqiO74sbTPAVyGyoq9hat8RgBvq+io0K0U6FaQQOa9dnE6V6iAyRC30O08MuI1r1MCFVFVlo34MQRW/po7+tgcGGm5QwSDJvDIf74GOYlu58rrGSJUNVWAV0zjSVXgyo0LGmCGCr6ZsalzjSNgabvHZahmekGJHp/Bzw/PzH2yOgWeGC56yh/GyIoWrG1kw1XlPAs2HH+AwAhyp1KNCnN+atdr4jees8L0JoqGzMOsfZigawopvwKp9RzeZBD2fLCUTbaYRpqgJM7SxKkpGX5fpHrYS47tkFwcQ1Xk00Ahfgzf3VylVB0iJ/AcOtYNjl5M31sLpYoLdplynw/aA74AtMNevTv7gWQXNY9Um+RpSh96OhnG8rzeD7vubqNcNhlmGd72bVdWv8aW6rKL/pdcLkPRLhkAh96hJV4KqZnC85+wmNi9Q7SOKnDHjsvRExBp8VSasdS1be9jpk7uRxwYBMxanitrL3J3NkGe8ZEF7ipqTmIOnbYyrtRXxHFggbryp76XzxSvHH1Pos5xeN61uLEQtBR9c2e9FFTP8iI0aLRmlVn0PMab7C0Y9f1sj3KaBOn6hy57q8gmP9lVrXMuGZRFFQiFrA/G9uApDHrtFNw==",
			privateKey:
				"""
				-----BEGIN OPENSSH PRIVATE KEY-----
				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
				qQzgjRdiL/0AAAAAAQID
				-----END OPENSSH PRIVATE KEY-----
				"""),
	];

	private readonly Dictionary<string, SshKeyOptions> Options = new()
	{
		["qhhvy45adllqzjal5w3luvl7lm"] = new() { EnableForMachines = ["*"] },
		["u5utdrdxlqu5qv556ewil425xi"] = new() { EnableForMachines = ["*"] },
		["kqb42mkf5c3maeprgkg6clb3jm"] = new() { EnableForMachines = ["*"] },
		["rgatdoo4zobwyhtsivnod636hu"] = new() { EnableForMachines = [] },
	};

	private Dictionary<string, SshKey> PrivateKeys = new();
	public IReadOnlyList<SshKey> PublicKeys { get; private set; } = Array.Empty<SshKey>();

	ValueTask<SshKey> ISshKeyStore.GetPrivateKey(SshKey publicKey, CancellationToken ct)
	{
		if (!PrivateKeys.TryGetValue(publicKey.Id, out var key))
			return default;
		return new(key);
	}

	async Task ISshKeyStore.SyncKeys(CancellationToken ct)
	{
		await Task.Delay(3000, ct);

		PrivateKeys.Clear();
		foreach (var key in InitialKeys)
			PrivateKeys[key.Id] = key;

		PublicKeys = InitialKeys
			.Select(x => x with { PrivateKey = null })
			.ToList();
	}

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
			Options.Remove(id);
		else
			Options[id] = options.Value;
	}

	Task ISshKeyOptionsStore.SyncKeyOptions(CancellationToken ct)
	{
		return Task.CompletedTask;
	}
}
