using DynamicData.Kernel;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KeyWarden;

internal class AgentK : ISshAgentHandler
{
	private readonly ISshKeyStore KeyStore;

	public AgentK(ISshKeyStore keyStore)
	{
		KeyStore = keyStore;
	}

	public ValueTask<IReadOnlyList<SshKey>> GetPublicKeys(ClientInfo info, CancellationToken ct)
	{
		return new(KeyStore.PublicKeys);
	}

	public Func<SshKey, ClientInfo, CancellationToken, Task<bool>>? HandleAuthPrompt { get; set; }

	public async ValueTask<SshKey> GetPrivateKey(SshKey publicKey, ClientInfo info, CancellationToken ct)
	{
		if (HandleAuthPrompt is null)
			return default;

		var publicKeyFull = KeyStore.PublicKeys
			.Where(k => k.PublicKey.Span.SequenceEqual(publicKey.PublicKey.Span))
			.FirstOrDefault();

		if (publicKeyFull.IsEmpty)
			publicKeyFull = publicKey;

		if (!await HandleAuthPrompt(publicKeyFull, info, ct))
			return default;
		return await KeyStore.GetPrivateKey(publicKey, ct);
	}
}
