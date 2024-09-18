using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Avalonia.Threading;

using DynamicData;

using Keyden.ViewModels;
using Keyden.Views;

namespace Keyden;

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
	private KeydenSettings Settings { get; }

	private readonly ISshKeyStore KeyStore;
	private readonly ISshKeyOptionsStore KeyOptionsStore;
	private readonly ISystemServices SystemServices;

	public event Action<ActivityItem>? NewActivity;

	public AgentK(
		KeydenSettings settings,
		ISshKeyStore keyStore,
		ISshKeyOptionsStore keyOptionsStore,
		ISystemServices systemServices)
	{
		Settings = settings;
		KeyStore = keyStore;
		KeyOptionsStore = keyOptionsStore;
		SystemServices = systemServices;

		foreach (var key in KeyStore.PublicKeys)
		{
			var newKey = new ObservableSshKey(key.Id, key.Name, key.Fingerprint, key.PublicKeyText);
			var options = KeyOptionsStore.GetKeyOptions(key.Id);

			newKey.SetOptions(options ?? new());
			Keys.Add(newKey);
		}

		SystemServices.MachineLocked += UserActivityTracker_MachineLocked;
		MonitorIdleThread();
	}

	private void UserActivityTracker_MachineLocked(object? sender, EventArgs e)
	{
		foreach (var (id, keyInfo) in KeyInfos)
		{
			var opts = KeyOptionsStore.GetKeyOptions(id);

			if (opts?.RemainAuthorizedUntilLocked ?? true && keyInfo.AuthorizedAt is not null)
			{
				keyInfo.AuthorizedAt = null;

				NewActivity?.Invoke(new ActivityItem()
				{
					Icon = "fa-lock",
					Importance = ActivityImportance.Critical,
					Title = "Key deauthorized",
					Description = $"{keyInfo.Name} deauthorized due to the session being locked",
				});
			}

			if (opts?.RemainAuthenticatedUntilLocked ?? true && keyInfo.AuthenticatedAt is not null)
			{
				keyInfo.AuthenticatedAt = null;

				NewActivity?.Invoke(new ActivityItem()
				{
					Icon = "fa-lock",
					Importance = ActivityImportance.Critical,
					Title = "Key deauthenticated",
					Description = $"{keyInfo.Name} deauthenticated due to the session being locked",
				});
			}
		}
	}

	private async void MonitorIdleThread()
	{
		var cts = new CancellationTokenSource();
		Dispatcher.UIThread.ShutdownStarted += shuttingDown;
		void shuttingDown(object? sender, EventArgs e)
		{
			cts.Cancel();
		}

		try
		{
			while (!cts.IsCancellationRequested)
			{
				await Dispatcher.UIThread.AwaitWithPriority(Task.Delay(60_000, cts.Token), DispatcherPriority.Background);
				var idleFor = SystemServices.UserIdleDuration;

				foreach (var (id, keyInfo) in KeyInfos)
				{
					var opts = KeyOptionsStore.GetKeyOptions(id);

					if (opts?.RemainAuthorizedUntilUserInactivity ?? true && keyInfo.AuthorizedAt is not null)
					{
						var maxIdleDuration = opts?.RemainAuthorizedUntilUserInactivityFor ?? TimeSpan.Zero;
						if (idleFor > maxIdleDuration)
						{
							keyInfo.AuthorizedAt = null;

							NewActivity?.Invoke(new ActivityItem()
							{
								Icon = "fa-lock",
								Importance = ActivityImportance.Critical,
								Title = "Key deauthorized",
								Description = $"{keyInfo.Name} deauthorized due to user inactivity",
							});
						}
					}

					if (opts?.RemainAuthenticatedUntilUserInactivity ?? true && keyInfo.AuthenticatedAt is not null)
					{
						var maxIdleDuration = opts?.RemainAuthenticatedUntilUserInactivityFor ?? TimeSpan.Zero;
						if (idleFor > maxIdleDuration)
						{
							keyInfo.AuthenticatedAt = null;

							NewActivity?.Invoke(new ActivityItem()
							{
								Icon = "fa-lock",
								Importance = ActivityImportance.Critical,
								Title = "Key deauthenticated",
								Description = $"{keyInfo.Name} deauthenticated due to user inactivity",
							});
						}
					}
				}
			}
		}
		catch (OperationCanceledException) { }
	}

	public SortableObservableCollection<ObservableSshKey, string> Keys { get; } = new()
	{
		SortingSelector = k => k.Name,
	};

	public async Task SyncKeys()
	{
		try
		{
			await KeyStore.SyncKeys();
		}
		catch (BackendException ex)
		{
			NewActivity?.Invoke(new ActivityItem()
			{
				Icon = "fa-circle-exclamation",
				Importance = ActivityImportance.Critical,
				Title = "Backend error",
				Description = ex.Message,
			});

			if (await ExceptionWindow.Prompt(ex.Message) == ExceptionWindowResult.Abort)
				throw;
			return;
		}
		catch (Exception ex)
		{
			NewActivity?.Invoke(new ActivityItem()
			{
				Icon = "fa-circle-exclamation",
				Importance = ActivityImportance.Critical,
				Title = "Backend exception",
				Description = ex.Message.ToString(),
			});

			if (await ExceptionWindow.Prompt(ex.ToString()) == ExceptionWindowResult.Abort)
				throw;
			return;
		}

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
				KeyOptionsStore.SetKeyOptions(key.Id, null);
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

		// sync options
		foreach (var key in Keys)
		{
			if (!key.Modified)
				continue;
			KeyOptionsStore.SetKeyOptions(key.Id, key.GetOptions());
		}

		try
		{
			await KeyOptionsStore.SyncKeyOptions();
		}
		catch (BackendException ex)
		{
			NewActivity?.Invoke(new ActivityItem()
			{
				Icon = "fa-circle-exclamation",
				Importance = ActivityImportance.Critical,
				Title = "Backend error",
				Description = ex.Message,
			});

			if (await ExceptionWindow.Prompt(ex.Message) == ExceptionWindowResult.Abort)
				throw;
			return;
		}
		catch (Exception ex)
		{
			NewActivity?.Invoke(new ActivityItem()
			{
				Icon = "fa-circle-exclamation",
				Importance = ActivityImportance.Critical,
				Title = "Backend exception",
				Description = ex.Message.ToString(),
			});

			if (await ExceptionWindow.Prompt(ex.ToString()) == ExceptionWindowResult.Abort)
				throw;
			return;
		}

		Keys.RemoveMany(removedKeys);
		Keys.Add(newKeys);

		foreach (var key in Keys)
			key.SetOptions(KeyOptionsStore.GetKeyOptions(key.Id) ?? new());

		NewActivity?.Invoke(new ActivityItem()
		{
			Icon = "fa-sync",
			Importance = ActivityImportance.Normal,
			Title = "Synced with password manager",
			Description = $"Got {newKeys.Count} new keys and removed {removedKeys.Count} keys",
		});
	}

	ValueTask<IReadOnlyList<SshKey>> ISshAgentHandler.GetPublicKeys(ClientInfo info, CancellationToken ct)
	{
		var mainProcess = info.MainProcess;
		var processChain = string.Join(", ", info.Processes
			.Until(p => p == mainProcess)
			.Select(static p => p.ProcessName)
			.CompactDuplicates());

		if (!string.IsNullOrEmpty(processChain))
			processChain = $" (via {processChain})";

		NewActivity?.Invoke(new ActivityItem()
		{
			Icon = "fa-magnifying-glass",
			Importance = ActivityImportance.Normal,
			Title = "Keys queried",
			Description = $"{info.ApplicationName}{processChain} queried the available keys",
		});

		var enabledKeys = KeyStore.PublicKeys
			.Select(k => (key: k, options: KeyOptionsStore.GetKeyOptions(k.Id)))
			.Where(x => x.options is not null &&
			(
				x.options.Value.EnableForMachines.Contains(Environment.MachineName) ||
				x.options.Value.EnableForMachines.Contains("*")
			))
			.Select(x => x.key)
			.ToList();

		return new(enabledKeys);
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
		public required string Name;
		public DateTime? LastUsed;
		public DateTime? AuthorizedAt;
		public DateTime? AuthenticatedAt;
	}
	private readonly Dictionary<string, KeyInfo> KeyInfos = new();


	private async ValueTask<AuthRequired> QueryAuth(SshKey key, SshKeyOptions options, ClientInfo clientInfo, CancellationToken ct)
	{
		var ret = AuthRequired.None;

		if (!KeyInfos.TryGetValue(key.Id, out var keyInfo))
			KeyInfos[key.Id]=  keyInfo = new KeyInfo() { Name = key.Name };

		var now = DateTime.UtcNow;
		TimeSpan? lastUsedAgo = null;
		if (keyInfo.LastUsed.HasValue)
			lastUsedAgo = (now - keyInfo.LastUsed.Value);

		if (options.RequireAuthorization)
		{
			if (!options.RemainAuthorized)
				ret |= AuthRequired.Authorization;
			else
			{
				bool expired =
					lastUsedAgo is null ||
					(DateTime.UtcNow - keyInfo.AuthorizedAt) > options.RemainAuthorizedFor;

				if (expired && keyInfo.AuthorizedAt is not null)
				{
					keyInfo.AuthorizedAt = null;

					NewActivity?.Invoke(new ActivityItem()
					{
						Icon = "fa-lock",
						Importance = ActivityImportance.Critical,
						Title = "Key deauthorized",
						Description = $"{keyInfo.Name} deauthorized due to timeout",
					});
				}

				if (options.RemainAuthorizedUntilKeyInactivity && keyInfo.AuthorizedAt is not null)
				{
					expired |=
						lastUsedAgo is null ||
						lastUsedAgo.Value > options.RemainAuthorizedUntilKeyInactivityFor;

					if (expired && keyInfo.AuthorizedAt is not null)
					{
						keyInfo.AuthorizedAt = null;

						NewActivity?.Invoke(new ActivityItem()
						{
							Icon = "fa-lock",
							Importance = ActivityImportance.Critical,
							Title = "Key deauthorized",
							Description = $"{keyInfo.Name} deauthorized due to key inactivity",
						});
					}
				}

				if (expired || keyInfo.AuthorizedAt is null)
					ret |= AuthRequired.Authorization;
			}
		}

		if (options.RequireAuthentication)
		{
			if (!options.RemainAuthenticated)
				ret |= AuthRequired.Authentication;
			else
			{
				bool expired =
					lastUsedAgo is null ||
					(DateTime.UtcNow - keyInfo.AuthenticatedAt) > options.RemainAuthenticatedFor;

				if (expired && keyInfo.AuthenticatedAt is not null)
				{
					keyInfo.AuthenticatedAt = null;

					NewActivity?.Invoke(new ActivityItem()
					{
						Icon = "fa-lock",
						Importance = ActivityImportance.Critical,
						Title = "Key deauthenticated",
						Description = $"{keyInfo.Name} deauthenticated due to timeout",
					});
				}

				if (options.RemainAuthenticatedUntilKeyInactivity && keyInfo.AuthenticatedAt is not null)
				{
					expired |=
						lastUsedAgo is null ||
						lastUsedAgo.Value > options.RemainAuthenticatedUntilKeyInactivityFor;

					if (expired && keyInfo.AuthenticatedAt is not null)
					{
						keyInfo.AuthenticatedAt = null;

						NewActivity?.Invoke(new ActivityItem()
						{
							Icon = "fa-lock",
							Importance = ActivityImportance.Critical,
							Title = "Key deauthenticated",
							Description = $"{keyInfo.Name} deauthenticated due to key inactivity",
						});
					}
				}

				if (expired || keyInfo.AuthenticatedAt is null)
					ret |= AuthRequired.Authentication;
			}
		}

		return ret;
	}

	private async ValueTask<SshKey> GotAuthResult(SshKey key, SshKeyOptions options, ClientInfo info, AuthResult result, CancellationToken ct)
	{
		if (!KeyInfos.TryGetValue(key.Id, out var keyInfo))
			KeyInfos[key.Id] = keyInfo = new KeyInfo() { Name = key.Name };

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

				keyInfo.AuthorizedAt = null;
				keyInfo.AuthenticatedAt = null;
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

		keyInfo.LastUsed = DateTime.UtcNow;
		if (result.FreshAuthorization)
			keyInfo.AuthorizedAt = DateTime.UtcNow;
		if (result.FreshAuthentication)
			keyInfo.AuthenticatedAt = DateTime.UtcNow;

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

		var keyEnabled =
			keyOptions.EnableForMachines.Contains(Environment.MachineName) ||
			keyOptions.EnableForMachines.Contains("*");
		if (!keyEnabled)
		{
			NewActivity?.Invoke(new ActivityItem()
			{
				Icon = "fa-gavel",
				Importance = ActivityImportance.Normal,
				Title = "Key disabled",
				Description = $"{info.ApplicationName}{processChain} requested access to a disabled private key",
			});

			return default;
		}

		// check for pre-authorization
		var authRequired = await QueryAuth(publicKey, keyOptions, info, ct);
		if (authRequired == AuthRequired.None)
			return await GotAuthResult(publicKey, keyOptions, info, new() { Success = true }, ct);

		await PromptQueue.WaitAsync(ct);

		// check again for pre-authorization, as a previous auth request may suffice
		authRequired = await QueryAuth(publicKey, keyOptions, info, ct);
		if (authRequired is AuthRequired.None)
			return await GotAuthResult(publicKey, keyOptions, info, new() { Success = true }, ct);

		using var @lock = new ScopedLock(PromptQueue);

		AuthPrompt? window = null;
		AuthResult result = new() { Success = true };

		try
		{
			if (authRequired.HasFlag(AuthRequired.Authentication))
			{
				window = new AuthPrompt(Settings, publicKey, info, authRequired, ct);
				window.Show();
				result = await window.Result;
				window.Topmost = false;
			}

			if (result.Success && authRequired.HasFlag(AuthRequired.Authentication))
			{
				var systemAuthResult = await SystemServices.TryAuthenticateUser();
				if (systemAuthResult.Success)
					result.FreshAuthentication = true;
				else
					result.Success = false;
			}

			return await GotAuthResult(publicKey, keyOptions, info, result, ct);
		}
		catch (BackendException ex)
		{
			window?.Close();
			NewActivity?.Invoke(new ActivityItem()
			{
				Icon = "fa-circle-exclamation",
				Importance = ActivityImportance.Critical,
				Title = "Backend error",
				Description = ex.Message,
			});

			if (await ExceptionWindow.Prompt(ex.Message, ct) == ExceptionWindowResult.Abort)
				throw;
			return default;
		}
		catch (Exception ex)
		{
			window?.Close();
			NewActivity?.Invoke(new ActivityItem()
			{
				Icon = "fa-circle-exclamation",
				Importance = ActivityImportance.Critical,
				Title = "Backend exception",
				Description = ex.Message.ToString(),
			});

			if (await ExceptionWindow.Prompt(ex.ToString(), ct) == ExceptionWindowResult.Abort)
				throw;
			return default;
		}
		finally
		{
			window?.Close();
		}
	}
}
