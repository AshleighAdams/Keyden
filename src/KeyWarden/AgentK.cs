using CommunityToolkit.Mvvm.ComponentModel;

using DynamicData;

using KeyWarden.ViewModels;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KeyWarden;

public partial class ObservableSshKey : ObservableObject
{
	[ObservableProperty]
	private string _Name = string.Empty;

	[ObservableProperty]
	private string _Fingerprint = string.Empty;

	[ObservableProperty]
	private string _PublicKey = string.Empty;
}

public class AgentK : ISshAgentHandler
{
	private readonly ISshKeyStore KeyStore;

	public event Action<ActivityItem>? NewActivity;
	public event PropertyChangedEventHandler? PropertyChanged;

	public AgentK(ISshKeyStore keyStore)
	{
		KeyStore = keyStore;
	}

	public ObservableCollection<ObservableSshKey> Keys { get; } = new();

	public async Task SyncKeys()
	{
		await KeyStore.SyncKeys();

		// check for removed keys
		var removedKeys = new List<ObservableSshKey>();
		foreach (var key in Keys)
		{
			var matchedKey = KeyStore.PublicKeys
				.Where(k => k.Name == key.Name || k.Fingerprint == key.Fingerprint)
				.FirstOrDefault();

			key.Name = matchedKey.Name;
			key.Fingerprint = matchedKey.Fingerprint;

			if (matchedKey.IsEmpty)
				removedKeys.Add(key);
		}

		// check for new keys
		var newKeys = new List<ObservableSshKey>();
		foreach (var key in KeyStore.PublicKeys)
		{
			var matchedKey = Keys
				.Where(k => k.Name == key.Name || k.Fingerprint == key.Fingerprint)
				.Any();

			if (!matchedKey)
			{
				newKeys.Add(new()
				{
					Name = key.Name,
					Fingerprint = key.Fingerprint,
					PublicKey = Encoding.UTF8.GetString(key.PublicKey.Span),
				});
			}
		}

		NewActivity?.Invoke(new ActivityItem()
		{
			Icon = "fa-sync",
			Importance = ActivityImportance.Normal,
			Title = "Synced with password manager",
			Description = $"Got {newKeys.Count} new keys and removed {removedKeys.Count} keys",
		});

		Keys.RemoveMany(removedKeys);
		Keys.Add(newKeys);
	}

	ValueTask<IReadOnlyList<SshKey>> ISshAgentHandler.GetPublicKeys(ClientInfo info, CancellationToken ct)
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

	async ValueTask<SshKey> ISshAgentHandler.GetPrivateKey(SshKey publicKey, ClientInfo info, CancellationToken ct)
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

		//var window = new AuthPrompt(publicKey, info, ct);
		//window.Show();

		if (false)//!await window.Result)
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
