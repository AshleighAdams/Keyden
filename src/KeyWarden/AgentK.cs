using CommunityToolkit.Mvvm.ComponentModel;

using DynamicData;

using KeyWarden.ViewModels;
using KeyWarden.Views;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KeyWarden;

public partial class ObservableSshKey : ObservableObject
{
	public ObservableSshKey(
		string name,
		string fingerprint,
		string publicKey,
		SshKeyOptions options)
	{
		_Name = name;
		_Fingerprint = fingerprint;
		_PublicKey = publicKey;

		_RequireAuthorization = options.RequireAuthorization;
		{
			_RemainAuthorized = options.RemainAuthorized;
			_RemainAuthorizedFor = options.RemainAuthorizedFor;
			{
				_RemainAuthorizedUntilKeyInactivity = options.RemainAuthorizedUntilKeyInactivity;
				_RemainAuthorizedUntilKeyInactivityFor = options.RemainAuthorizedUntilKeyInactivityFor;
				_RemainAuthorizedUntilUserInactivity = options.RemainAuthorizedUntilUserInactivity;
				_RemainAuthorizedUntilUserInactivityFor = options.RemainAuthorizedUntilUserInactivityFor;
				_RemainAuthorizedUntilLocked = options.RemainAuthorizedUntilLocked;
			}
		}
		_RequireAuthentication = options.RequireAuthentication;
		{
			_RemainAuthenticated = options.RemainAuthenticated;
			_RemainAuthenticatedFor = options.RemainAuthenticatedFor;
			{
				_RemainAuthenticatedUntilKeyInactivity = options.RemainAuthenticatedUntilKeyInactivity;
				_RemainAuthenticatedUntilKeyInactivityFor = options.RemainAuthenticatedUntilKeyInactivityFor;
				_RemainAuthenticatedUntilUserInactivity = options.RemainAuthenticatedUntilUserInactivity;
				_RemainAuthenticatedUntilUserInactivityFor = options.RemainAuthenticatedUntilUserInactivityFor;
				_RemainAuthenticatedUntilLocked = options.RemainAuthenticatedUntilLocked;
			}
		}
	}

	public SshKeyOptions GetOptions()
	{
		var ret = new SshKeyOptions();
		{
			ret.RequireAuthorization = RequireAuthorization;
			{
				ret.RemainAuthorized = RemainAuthorized;
				ret.RemainAuthorizedFor = RemainAuthorizedFor;
				{
					ret.RemainAuthorizedUntilKeyInactivity = RemainAuthorizedUntilKeyInactivity;
					ret.RemainAuthorizedUntilKeyInactivityFor = RemainAuthorizedUntilKeyInactivityFor;
					ret.RemainAuthorizedUntilUserInactivity = RemainAuthorizedUntilUserInactivity;
					ret.RemainAuthorizedUntilUserInactivityFor = RemainAuthorizedUntilUserInactivityFor;
					ret.RemainAuthorizedUntilLocked = RemainAuthorizedUntilLocked;
				}
			}
			ret.RequireAuthentication = RequireAuthentication;
			{
				ret.RemainAuthenticated = RemainAuthenticated;
				ret.RemainAuthenticatedFor = RemainAuthenticatedFor;
				{
					ret.RemainAuthenticatedUntilKeyInactivity = RemainAuthenticatedUntilKeyInactivity;
					ret.RemainAuthenticatedUntilKeyInactivityFor = RemainAuthenticatedUntilKeyInactivityFor;
					ret.RemainAuthenticatedUntilUserInactivity = RemainAuthenticatedUntilUserInactivity;
					ret.RemainAuthenticatedUntilUserInactivityFor = RemainAuthenticatedUntilUserInactivityFor;
					ret.RemainAuthenticatedUntilLocked = RemainAuthenticatedUntilLocked;
				}
			}
		}
		return ret;
	}

	[ObservableProperty]
	private string _Name;

	[ObservableProperty]
	private string _Fingerprint;

	[ObservableProperty]
	private string _PublicKey;

	[ObservableProperty]
	private bool _RequireAuthorization;

	[ObservableProperty]
	private bool _RemainAuthorized;
	[ObservableProperty]
	private TimeSpan _RemainAuthorizedFor;

	[ObservableProperty]
	private bool _RemainAuthorizedUntilKeyInactivity;
	[ObservableProperty]
	private TimeSpan _RemainAuthorizedUntilKeyInactivityFor;

	[ObservableProperty]
	private bool _RemainAuthorizedUntilUserInactivity;
	[ObservableProperty]
	private TimeSpan _RemainAuthorizedUntilUserInactivityFor;

	[ObservableProperty]
	private bool _RemainAuthorizedUntilLocked;

	[ObservableProperty]
	private bool _RequireAuthentication;

	[ObservableProperty]
	private bool _RemainAuthenticated;
	[ObservableProperty]
	private TimeSpan _RemainAuthenticatedFor;

	[ObservableProperty]
	private bool _RemainAuthenticatedUntilKeyInactivity;
	[ObservableProperty]
	private TimeSpan _RemainAuthenticatedUntilKeyInactivityFor;

	[ObservableProperty]
	private bool _RemainAuthenticatedUntilUserInactivity;
	[ObservableProperty]
	private TimeSpan _RemainAuthenticatedUntilUserInactivityFor;

	[ObservableProperty]
	private bool _RemainAuthenticatedUntilLocked;
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

			var keyOpts = new SshKeyOptions();
			if (!matchedKey)
				newKeys.Add(new(key.Name, key.Fingerprint, key.PublicKeyText, keyOpts));
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
		private SemaphoreSlim? Semaphore;
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
		public DateTime? LastUsed;
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
