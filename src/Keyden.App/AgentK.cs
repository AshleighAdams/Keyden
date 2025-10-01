using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

using Avalonia.Threading;

using DynamicData;

using Keyden.ViewModels;
using Keyden.Views;

namespace Keyden;

public class AgentK : ISshAgentHandler
{
	private readonly ISshKeyStore KeyStore;
	private readonly ISshKeyOptionsStore KeyOptionsStore;
	private readonly ISystemServices SystemServices;
	private readonly KeydenSettings Settings;

	public event Action<ActivityItem>? NewActivity;

	public AgentK(
		ISshKeyStore keyStore,
		ISshKeyOptionsStore keyOptionsStore,
		ISystemServices systemServices,
		KeydenSettings settings)
	{
		KeyStore = keyStore;
		KeyOptionsStore = keyOptionsStore;
		SystemServices = systemServices;
		Settings = settings;

		foreach (var key in KeyStore.PublicKeys)
		{
			var newKey = new ObservableSshKey(key.Id, key.Name, key.Fingerprint, key.PublicKeyText);
			var options = KeyOptionsStore.GetKeyOptions(key.Id);

			newKey.SetOptions(options ?? new());
			Keys.Add(newKey);
		}

		SystemServices.MachineLocked += UserActivityTracker_MachineLocked;
		MonitorIdleThread();

		Settings.PropertyChanged += Settings_PropertyChanged;
	}

	private void Settings_PropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(Settings.PipePath))
		{
			PipePathChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	public static string DefaultPipePath
	{
		get
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				return "openssh-ssh-agent";

			if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
			{
				var uid = Unix.Getuid().ToString(CultureInfo.InvariantCulture);
				var socketDirectory = $"/run/user/{uid}";

				// check for legacy linux systems
				if (!Directory.Exists(socketDirectory))
					socketDirectory = Directory.Exists("/run") ? "/run" : "/var/run";

				if (!Directory.Exists(socketDirectory))
					return "keyden-ssh-agent";

				socketDirectory += "/keyden";
				var socketPath = $"{socketDirectory}/ssh-agent.sock";

				if (!Directory.Exists(socketDirectory))
					Directory.CreateDirectory(socketDirectory);
				return socketPath;
			}

			if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{
				var uid = Unix.Getuid().ToString(CultureInfo.InvariantCulture);
				return $"/var/run/{uid}-keyden-ssh-agent.sock";
			}

			return "keyden-ssh-agent";
		}
	}

	public string PipePath => string.IsNullOrEmpty(Settings.PipePath) ? DefaultPipePath : Settings.PipePath;
	public event EventHandler<EventArgs>? PipePathChanged;
	public Exception? ListenException { get; set; }

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

	public void AddActivity(string title, string description, string icon = "fa-circle-info", ActivityImportance importance = ActivityImportance.Normal)
	{
		NewActivity?.Invoke(new ActivityItem()
		{
			Title = title,
			Description = description,
			Icon = icon,
			Importance = importance,
		});
	}

	private AuthRequired QueryAuth(SshKey key, SshKeyOptions options, ClientInfo clientInfo)
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

		var authRequired = QueryAuth(publicKey, keyOptions, info);

		// check for pre-authorization
		if (authRequired == AuthRequired.None)
		{
			SystemServices.NotifyPreauthorizedKey(info, publicKey);
			return await GotAuthResult(publicKey, keyOptions, info, new() { Success = true }, ct);
		}

		await PromptQueue.WaitAsync(ct);
		using var @lock = new ScopedLock(PromptQueue);

		// refresh the required auth, a previous reuqest could've changed this, and some time has passed since
		authRequired = QueryAuth(publicKey, keyOptions, info);

		// check again for pre-authorization
		if (authRequired is AuthRequired.None)
		{
			SystemServices.NotifyPreauthorizedKey(info, publicKey);
			return await GotAuthResult(publicKey, keyOptions, info, new() { Success = true }, ct);
		}

		try
		{
			var result = await SystemServices.TryAuthUser(authRequired, info, publicKey, ct);
			return await GotAuthResult(publicKey, keyOptions, info, result, ct);
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

			if (await ExceptionWindow.Prompt(ex.Message, ct) == ExceptionWindowResult.Abort)
				throw;
			return default;
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

			if (await ExceptionWindow.Prompt(ex.ToString(), ct) == ExceptionWindowResult.Abort)
				throw;
			return default;
		}
	}
}
