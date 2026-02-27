using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Keyden.OnePassword.SDK.Types;

namespace Keyden.OnePassword.SDK;

static file class TaskRunMarshaller
{
	public static YieldTask Yield()
	{
		return new();
	}

	public struct YieldTask
	{
		public YieldAwaiter GetAwaiter()
		{
			return new();
		}
	}

	public struct YieldAwaiter : ICriticalNotifyCompletion
	{
		public bool IsCompleted => false;
		public void GetResult() { }

		private static ContextCallback? Runner { get; set; }
		public void OnCompleted(Action continuation)
		{
			[DebuggerNonUserCode]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static void executionContextRunner(object obj) => (obj as Action)!();
			Runner ??= executionContextRunner!;

			var ctx = ExecutionContext.Capture();
			if (ctx is not null)
				Task.Run(() => ExecutionContext.Run(ctx, Runner, continuation));
			else
				Task.Run(() => continuation());

		}

		public void UnsafeOnCompleted(Action continuation)
		{
			Task.Run(() => continuation());
		}
	}
}

public partial class OnePassSdk : IDisposable, IAsyncDisposable
{
	private OnePassSdkConfig Config;
	public string? AccountName { get; set; }

	private record struct ConnectionInformation(int ClientId, string AccountName);
	private ConnectionInformation? ConnectionInfo { get; set; }

	public OnePassSdk(OnePassSdkConfig config)
	{
		Config = config;
	}

	private readonly object ConnectLock = new();
	private void Connect()
	{
		lock (ConnectLock)
		{
			if (AccountName is null)
				throw new InvalidOperationException("Must assign an account name prior to use");

			var accountName = AccountName;
			var configJson = OnePassSdkConfig.ToJson(Config);
			var initResponse = OnePassSdkInterop.CallSharedLibrary(new()
			{
				Kind = OnePassSdkInterop.PayloadKind.InitClient,
				AccountName = accountName,
				Payload = configJson,
			});

			var clientId = int.Parse(initResponse.Payload, CultureInfo.InvariantCulture);

			ConnectionInfo = new(clientId, accountName);
		}
	}

	private void Disconnect()
	{
		lock (ConnectLock)
		{
			if (!ConnectionInfo.HasValue)
				return;

			try
			{
				_ = OnePassSdkInterop.CallSharedLibrary(new()
				{
					Kind = OnePassSdkInterop.PayloadKind.ReleaseClient,
					AccountName = AccountName ?? string.Empty,
					Payload = ConnectionInfo.Value.ClientId.ToString(null, CultureInfo.InvariantCulture),
				});
			}
			catch { }

			ConnectionInfo = null;
		}
	}

	private bool Disposed;
	protected virtual void Dispose(bool disposing)
	{
		if (!Disposed)
		{
			Disconnect();
			Disposed = true;
		}
	}

	private string Invoke(string name, [StringSyntax("json")] string parameters)
	{
		bool retry;
		int tryCount = 0;
		do
		{
			retry = false;
			tryCount++;
			try
			{
				if (!ConnectionInfo.HasValue)
					Connect();
				if (!ConnectionInfo.HasValue)
					throw new OnePassSdkChannelClosedException();

				var connectionInfo = ConnectionInfo.Value;
				return OnePassSdkInterop.Invoke(connectionInfo.AccountName, new()
				{
					Invocation = new()
					{
						ClientId = connectionInfo.ClientId,
						Parameters = new()
						{
							Name = name,
							Parameters = parameters,
						}
					},
				});
			}
			catch (OnePassSdkChannelConnectionDroppedException)
			{
				Disconnect();
				retry = true;
			}
			catch (OnePassSdkDesktopSessionExpiredException)
			{
				Disconnect();
				retry = true;
			}
		} while (retry && tryCount <= 3);

		throw new OnePassSdkChannelClosedException();
	}

	public string ResolveSecret(string secretReference)
	{
		var payload = ResolveSecretPayload.ToJson(new()
		{
			SecretReference = secretReference,
		});

		var response = Invoke("SecretsResolve", payload);
		return JsonSerializer.Deserialize(response, OnePassTypesJsonContext.Default.String)
			?? throw new JsonException($"Failed to parse params: {response}");
	}

	public async Task<string> ResolveSecretAsync(string secretReference)
	{
		await TaskRunMarshaller.Yield();
		return ResolveSecret(secretReference);
	}

	public ResolveSecretsResponse ResolveSecrets(IReadOnlyList<string> secretReferences)
	{
		var payload = ResolveSecretsPayload.ToJson(new()
		{
			SecretReferences = secretReferences,
		});
		var response = Invoke("SecretsResolveAll", payload);
		return ResolveSecretsResponse.FromJson(response);
	}

	public async Task<ResolveSecretsResponse> ResolveSecretsAsync(IReadOnlyList<string> secretReferences)
	{
		await TaskRunMarshaller.Yield();
		return ResolveSecrets(secretReferences);
	}

	public void ValidateSecretReference(string secretReference)
	{
		var payload = ValidateSecretReferencePayload.ToJson(new()
		{
			SecretReference = secretReference,
		});

		_ = Invoke("SecretsResolve", payload);
	}

	public async Task ValidateSecretReferenceAsync(string secretReference)
	{
		await TaskRunMarshaller.Yield();
		ValidateSecretReference(secretReference);
	}

	public GeneratePasswordResponse GeneratePassword(PasswordRecipe recipe)
	{
		throw new NotImplementedException();
	}

	public IReadOnlyList<GetVaultResponse> GetAllVaults()
	{
		var res = Invoke("VaultsList", @"{""params"": null}");
		return JsonSerializer.Deserialize(res, OnePassTypesJsonContext.Default.IReadOnlyListGetVaultResponse) ?? [];
	}

	public async Task<IReadOnlyList<GetVaultResponse>> GetAllVaultsAsync()
	{
		await TaskRunMarshaller.Yield();
		return GetAllVaults();
	}

	public Item UpdateItem(Item item)
	{
		var res = Invoke("ItemsPut", ItemsPutPayload.ToJson(new()
		{
			Item = item,
		}));
		return Item.FromJson(res);
	}

	public async Task<Item> UpdateItemAsync(Item item)
	{
		await TaskRunMarshaller.Yield();
		return UpdateItem(item);
	}

	public void DeleteItem(string vaultId, string itemId)
	{
		_ = Invoke("ItemsDelete", DeleteItemPayload.ToJson(new()
		{
			VaultId = vaultId,
			ItemId = itemId,
		}));
	}

	public async Task DeleteItemAsync(string vaultId, string itemId)
	{
		await TaskRunMarshaller.Yield();
		DeleteItem(vaultId, itemId);
	}

	public ItemsDeleteAllResponse DeleteItems(string vaultId, IReadOnlyList<string> itemIds)
	{
		var res = Invoke("ItemsDeleteAll", DeleteAllItemsPayload.ToJson(new()
		{
			VaultId = vaultId,
			ItemIds = itemIds,
		}));
		return ItemsDeleteAllResponse.FromJson(res);
	}

	public async Task<ItemsDeleteAllResponse> DeleteItemsAsync(string vaultId, IReadOnlyList<string> itemIds)
	{
		await TaskRunMarshaller.Yield();
		return DeleteItems(vaultId, itemIds);
	}

	public Item CreateItem(ItemCreateParams itemParam)
	{
		var res = Invoke("ItemsCreate", ItemsCreatePayload.ToJson(new() { Params = itemParam }));
		return Item.FromJson(res);
	}

	public async Task<Item> CreateItemAsync(ItemCreateParams itemParam)
	{
		await TaskRunMarshaller.Yield();
		return CreateItem(itemParam);
	}

	public ItemsUpdateAllResponse CreateItems(string vaultId, IReadOnlyList<ItemCreateParams> itemsParam)
	{
		var res = Invoke("ItemsCreateAll", ItemsCreateAllPayload.ToJson(new()
		{
			VaultId = vaultId,
			Params = itemsParam,
		}));
		return ItemsUpdateAllResponse.FromJson(res);
	}

	public async Task<ItemsUpdateAllResponse> CreateItemsAsync(string vaultId, IReadOnlyList<ItemCreateParams> itemsParam)
	{
		await TaskRunMarshaller.Yield();
		return CreateItems(vaultId, itemsParam);
	}

	// https://github.com/1Password/onepassword-sdk-python/blob/main/src/onepassword/items.py#L91
	public Item GetItem(string vaultId, string itemId)
	{
		var res = Invoke("ItemsGet", GetItemsPayload.ToJson(new()
		{
			VaultId = vaultId,
			ItemId = itemId,
		}));
		return Item.FromJson(res);
	}

	public async Task<Item> GetItemAsync(string vaultId, string itemId)
	{
		await TaskRunMarshaller.Yield();
		return GetItem(vaultId, itemId);
	}

	public ItemsGetAllResponse GetItems(string vaultId, IReadOnlyList<string> itemIds)
	{
		var res = Invoke("ItemsGetAll", GetAllItemsPayload.ToJson(new()
		{
			VaultId = vaultId,
			ItemIds = itemIds,
		}));
		return ItemsGetAllResponse.FromJson(res);
	}

	public async Task<ItemsGetAllResponse> GetItemsAsync(string vaultId, IReadOnlyList<string> itemIds)
	{
		await TaskRunMarshaller.Yield();
		return GetItems(vaultId, itemIds);
	}

	public IReadOnlyList<ItemOverview> ListItems(string vaultId, IReadOnlyList<ItemListFilter> filters)
	{
		var res = Invoke("ItemsList", ListItemsPayload.ToJson(new()
		{
			VaultId = vaultId,
			Filters = filters,
		}));
		return JsonSerializer.Deserialize(res, OnePassTypesJsonContext.Default.IReadOnlyListItemOverview)
			?? [];
	}

	public async Task<IReadOnlyList<ItemOverview>> ListItemsAsync(string vaultId, IReadOnlyList<ItemListFilter> filters)
	{
		await TaskRunMarshaller.Yield();
		return ListItems(vaultId, filters);
	}

	~OnePassSdk()
	{
		Dispose(disposing: false);
	}

	public void Dispose()
	{
		// Do not change this code. UpdateItem cleanup code in 'Dispose(bool disposing)' method
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	async ValueTask IAsyncDisposable.DisposeAsync()
	{
		await TaskRunMarshaller.Yield();
		Dispose(true);
		GC.SuppressFinalize(this);
	}
}
