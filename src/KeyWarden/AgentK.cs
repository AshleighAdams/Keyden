using CommunityToolkit.Mvvm.ComponentModel;

using DynamicData;

using KeyWarden.ViewModels;
using KeyWarden.Views;

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

	[ObservableProperty]
	private SshKeyOptions _Options;
}

public class AgentK : ISshAgentHandler
{
	private readonly ISshKeyStore KeyStore;
	private readonly ISshKeyOptionsStore KeyOptionsStore;

	public event Action<ActivityItem>? NewActivity;

	public AgentK(ISshKeyStore keyStore, ISshKeyOptionsStore keyOptionsStore)
	{
		KeyStore = keyStore;
		KeyOptionsStore = keyOptionsStore;
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
					PublicKey = key.PublicKeyText,
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

	private SemaphoreSlim PromptQueue { get; } = new SemaphoreSlim(1);

	public struct ScopedLock : IDisposable
	{
		private SemaphoreSlim? Semaphore { get; set; }
		public ScopedLock(SemaphoreSlim semaphore) =>
			Semaphore = semaphore;
		public void Dispose()
		{
			Semaphore?.Release();
			Semaphore = null;
		}
	}

	private class KeyInfo
	{
		public DateTime? LastUsed { get; set; }
	}
	private readonly Dictionary<string, KeyInfo> KeyInfos = new();

	async ValueTask<SshKey> ISshAgentHandler.GetPrivateKey(ReadOnlyMemory<byte> publicKeyBlob, ClientInfo info, CancellationToken ct)
	{
		var mainProcess = info.MainProcess;
		var processChain = string.Join(", ", info.Processes
			.Until(p => p == mainProcess)
			.Select(static p => p.ProcessName));

		if (!string.IsNullOrEmpty(processChain))
			processChain = $" (via {processChain})";

		var publicKey = KeyStore.PublicKeys
			.Where(k => k.PublicKey.Span.SequenceEqual(publicKeyBlob.Span))
			.FirstOrDefault();

		if (publicKey.IsEmpty)
		{
			NewActivity?.Invoke(new ActivityItem()
			{
				Icon = "fa-ban", // maybe gavel
				Importance = ActivityImportance.Normal,
				Title = "Auth error",
				Description = $"{info.ApplicationName}{processChain} requested access to unknown private key",
			});

			return default;
		}

		await PromptQueue.WaitAsync(ct);
		using var @lock = new ScopedLock(PromptQueue);

		var window = new AuthPrompt(publicKey, info, ct);
		window.Show();

		if (!await window.Result)
		{
			NewActivity?.Invoke(new ActivityItem()
			{
				Icon = "fa-ban", // maybe gavel
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
				Icon = "fa-passport", // authenticated icon
				//Icon = "fa-id-card", // authenticated icon
				//Icon = "fa-fingerprint", // another possible authenticated icon, perhaps with biometrics only

				//Icon = "fa-key", // possible authorized icon
				//Icon = "fa-unlock", // possible authorized icon or based on not timing out
				//Icon = "fa-lock-open", // possible authorized icon or based on not timing out

				//Icon = "fa-bell-slash", // key does not require authorization nor authentication
				// Icon = "fa-hourglass", // key authorized based on not timing out
				//Icon = "fa-hourglass-end", // key timed out?
				Importance = ActivityImportance.Normal,
				Title = "Auth success",
				Description = $"{info.ApplicationName}{processChain} was granted access to the key {publicKey.Name}",
			});

			return await KeyStore.GetPrivateKey(publicKey, ct);
		}
	}
}
