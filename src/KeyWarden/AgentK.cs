using KeyWarden.ViewModels;
using KeyWarden.Views;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KeyWarden;

public class AgentK : ISshAgentHandler
{
	private readonly ISshKeyStore KeyStore;
	public event Action<ActivityItem>? NewActivity;

	public AgentK(ISshKeyStore keyStore)
	{
		KeyStore = keyStore;
	}

	public ValueTask<IReadOnlyList<SshKey>> GetPublicKeys(ClientInfo info, CancellationToken ct)
	{
		var mainProcess = info.MainProcess;
		var processChain = string.Join(", ", info.Processes
			.Until(p => p == mainProcess)
			.Select(static p => p.ProcessName));

		if (!string.IsNullOrEmpty(processChain))
			processChain = $" (via {processChain})";

		NewActivity?.Invoke(new ActivityItem()
		{
			Icon = "fa-magnifying-glass",
			Importance = ActivityImportance.Normal,
			Title = "Keys queried",
			Description = $"{info.ApplicationName}{processChain} queried the available keys",
		});

		return new(KeyStore.PublicKeys);
	}

	public async ValueTask<SshKey> GetPrivateKey(SshKey publicKey, ClientInfo info, CancellationToken ct)
	{
		var mainProcess = info.MainProcess;
		var processChain = string.Join(", ", info.Processes
			.Until(p => p == mainProcess)
			.Select(static p => p.ProcessName));

		if (!string.IsNullOrEmpty(processChain))
			processChain = $" (via {processChain})";

		publicKey = KeyStore.PublicKeys
			.Where(k => k.PublicKey.Span.SequenceEqual(publicKey.PublicKey.Span))
			.FirstOrDefault(publicKey);

		var window = new AuthPrompt(publicKey, info, ct);
		window.Show();

		if (!await window.Result)
		{
			NewActivity?.Invoke(new ActivityItem()
			{
				Icon = "fa-lock",
				Importance = ActivityImportance.Normal,
				Title = "Auth fail",
				Description = $"{info.ApplicationName}{processChain} was denied access to the key {publicKey.Name}",
			});

			return default;
		}
		else
		{
			NewActivity?.Invoke(new ActivityItem()
			{
				Icon = "fa-unlock",
				Importance = ActivityImportance.Normal,
				Title = "Auth success",
				Description = $"{info.ApplicationName}{processChain} was granted access to the key {publicKey.Name}",
			});

			return await KeyStore.GetPrivateKey(publicKey, ct);
		}
	}
}
