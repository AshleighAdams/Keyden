using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

using Avalonia.Controls;

using CommunityToolkit.Mvvm.ComponentModel;

using DynamicData;

using KeyWarden.ViewModels;
using KeyWarden.Views;

namespace KeyWarden;

public partial class ObservableSshKey : ObservableObject
{
	public ObservableSshKey(
		string id,
		string name,
		string fingerprint,
		string publicKey)
	{
		Id = id;
		_Name = name;
		_Fingerprint = fingerprint;
		_PublicKey = publicKey;
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
	public void SetOptions(SshKeyOptions options)
	{
		RequireAuthorization = options.RequireAuthorization;
		{
			RemainAuthorized = options.RemainAuthorized;
			RemainAuthorizedFor = options.RemainAuthorizedFor;
			{
				RemainAuthorizedUntilKeyInactivity = options.RemainAuthorizedUntilKeyInactivity;
				RemainAuthorizedUntilKeyInactivityFor = options.RemainAuthorizedUntilKeyInactivityFor;
				RemainAuthorizedUntilUserInactivity = options.RemainAuthorizedUntilUserInactivity;
				RemainAuthorizedUntilUserInactivityFor = options.RemainAuthorizedUntilUserInactivityFor;
				RemainAuthorizedUntilLocked = options.RemainAuthorizedUntilLocked;
			}
		}
		RequireAuthentication = options.RequireAuthentication;
		{
			RemainAuthenticated = options.RemainAuthenticated;
			RemainAuthenticatedFor = options.RemainAuthenticatedFor;
			{
				RemainAuthenticatedUntilKeyInactivity = options.RemainAuthenticatedUntilKeyInactivity;
				RemainAuthenticatedUntilKeyInactivityFor = options.RemainAuthenticatedUntilKeyInactivityFor;
				RemainAuthenticatedUntilUserInactivity = options.RemainAuthenticatedUntilUserInactivity;
				RemainAuthenticatedUntilUserInactivityFor = options.RemainAuthenticatedUntilUserInactivityFor;
				RemainAuthenticatedUntilLocked = options.RemainAuthenticatedUntilLocked;
			}
		}
	}

	public string Id { get; }

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

public struct AuthResult
{
	public bool Success { get; set; }
	public bool Rejected { get; set; }
	public bool FreshAuthorization { get; set; }
	public bool FreshAuthentication { get; set; }
}

[Flags]
public enum AuthRequired
{
	None,
	Authorization,
	Authentication,
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
				.Where(k => k.Id == key.Id)
				.FirstOrDefault();

			key.Name = matchedKey.Name;
			key.Fingerprint = matchedKey.Fingerprint;

			if (matchedKey.IsEmpty)
			{
				removedKeys.Add(key);
				KeyOptionsStore.SetKeyOptions(matchedKey.Id, null);
			}
		}

		bool syncedNewOptions = false;
		// check for new keys
		var newKeys = new List<ObservableSshKey>();
		foreach (var key in KeyStore.PublicKeys)
		{
			var matchedKey = Keys
				.Where(k => k.Id == key.Id)
				.Any();

			if (!matchedKey)
			{
				// may have some some new key options to match
				if (!syncedNewOptions)
				{
					syncedNewOptions = true;
					await KeyOptionsStore.SyncKeyOptions();
				}

				var newKey = new ObservableSshKey(key.Id, key.Name, key.Fingerprint, key.PublicKeyText);
				newKeys.Add(newKey);
			}
		}

		NewActivity?.Invoke(new ActivityItem()
		{
			Icon = "fa-sync",
			Importance = ActivityImportance.Normal,
			Title = "Synced with password manager",
			Description = $"Got {newKeys.Count} new keys and removed {removedKeys.Count} keys",
		});

		// sync options
		foreach (var key in Keys)
		{
			if (newKeys.Contains(key))
				continue;
			KeyOptionsStore.SetKeyOptions(key.Id, key.GetOptions());
		}
		await KeyOptionsStore.SyncKeyOptions();

		Keys.RemoveMany(removedKeys);
		Keys.Add(newKeys);

		foreach (var key in Keys)
			key.SetOptions(KeyOptionsStore.GetKeyOptions(key.Id) ?? new());
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


	private async ValueTask<AuthRequired> QueryAuth(SshKey key, SshKeyOptions options, ClientInfo info, CancellationToken ct)
	{
		var ret = AuthRequired.None;

		if (options.RequireAuthorization)
		{
			if (!options.RemainAuthorized)
			{
				ret |= AuthRequired.Authorization;
			}
		}

		if (options.RequireAuthentication)
		{
			if (!options.RemainAuthenticated)
			{
				ret |= AuthRequired.Authentication;
			}
		}

		return ret;
	}

	private async ValueTask<SshKey> GotAuthResult(SshKey key, SshKeyOptions options, ClientInfo info, AuthResult result, CancellationToken ct)
	{

		if (!result.Success)
		{
			if (result.Rejected)
			{
				NewActivity?.Invoke(new ActivityItem()
				{
					Icon = "fa-ban", // maybe gavel
					Importance = ActivityImportance.Normal,
					Title = "Auth fail",
					Description = $"{info.ApplicationName} was denied access to the key {key.Name}",
				});
			}
			return default;
		}

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
			Description = $"{info.ApplicationName} was granted access to the key {key.Name}",
		});

		return await KeyStore.GetPrivateKey(key, ct);
	}

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

		var keyOptions = KeyOptionsStore.GetKeyOptions(publicKey.Id) ?? new();

		// check for pre-authorization
		var authRequired = await QueryAuth(publicKey, keyOptions, info, ct);
		if (authRequired == AuthRequired.None)
			return await GotAuthResult(publicKey, keyOptions, info, new(){ Success = true }, ct);

		await PromptQueue.WaitAsync(ct);

		// check again for pre-authorization, as a previous auth request may suffice
		authRequired = await QueryAuth(publicKey, keyOptions, info, ct);
		if (authRequired is AuthRequired.None)
			return await GotAuthResult(publicKey, keyOptions, info, new() { Success = true }, ct);
		
		using var @lock = new ScopedLock(PromptQueue);

		AuthPrompt? window = null;
		AuthResult result = new() { Success = true };

		if (authRequired.HasFlag(AuthRequired.Authentication))
		{
			window = new AuthPrompt(publicKey, info, authRequired, ct);
			window.Show();
			result = await window.Result;
		}

		if (result.Success && authRequired.HasFlag(AuthRequired.Authentication))
		{
			// TODO: authentication
			await Task.Delay(3000);
			result.FreshAuthentication = true;
		}
		window?.Close();

		return await GotAuthResult(publicKey, keyOptions, info, result, ct);
	}
}
