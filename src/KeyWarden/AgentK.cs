using KeyWarden.ViewModels;
using KeyWarden.Views;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KeyWarden;

public class AgentK : ISshAgentHandler
{
	private readonly ISshKeyStore KeyStore;
	public ObservableCollection<ActivityItem> Activities { get; } = new();

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
		publicKey = KeyStore.PublicKeys
			.Where(k => k.PublicKey.Span.SequenceEqual(publicKey.PublicKey.Span))
			.FirstOrDefault(publicKey);

		var window = new AuthPrompt(publicKey, info, ct);
		window.Show();

		if (!await window.Result)
			return default;

		return await KeyStore.GetPrivateKey(publicKey, ct);
	}
}
