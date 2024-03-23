using System;
using System.Collections.Generic;
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

	public async ValueTask<SshKey> GetPrivateKey(SshKey publicKey, ClientInfo info, CancellationToken ct)
	{
		return await KeyStore.GetPrivateKey(publicKey, ct);
	}
}
