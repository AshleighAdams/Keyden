using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Keyden.OnePassword.SDK.Types;

[JsonSerializable(typeof(OnePassSdkConfig))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(ResolveSecretPayload))]
[JsonSerializable(typeof(ResolveSecretsPayload))]
[JsonSerializable(typeof(ResolveSecretsResponse))]
[JsonSerializable(typeof(ValidateSecretReferencePayload))]
[JsonSerializable(typeof(PasswordRecipe))]
[JsonSerializable(typeof(GeneratePasswordResponse))]
[JsonSerializable(typeof(GetVaultResponse))]
[JsonSerializable(typeof(IReadOnlyList<GetVaultResponse>))]
[JsonSerializable(typeof(ItemOverview))]
[JsonSerializable(typeof(ItemsGetAllResponse))]
[JsonSerializable(typeof(IReadOnlyList<ItemOverview>))]
[JsonSerializable(typeof(ItemCreateParams))]
[JsonSerializable(typeof(ItemsUpdateAllResponse))]
[JsonSerializable(typeof(IReadOnlyList<ItemCreateParams>))]
[JsonSerializable(typeof(ItemsPutPayload))]
[JsonSerializable(typeof(GetItemPayload))]
[JsonSerializable(typeof(GetItemsPayload))]
[JsonSerializable(typeof(GetAllItemsPayload))]
[JsonSerializable(typeof(ListItemsPayload))]
[JsonSerializable(typeof(DeleteItemPayload))]
[JsonSerializable(typeof(DeleteAllItemsPayload))]
[JsonSerializable(typeof(ItemsDeleteAllResponse))]
[JsonSerializable(typeof(ItemsCreateAllPayload))]
[JsonSerializable(typeof(ItemsCreatePayload))]
internal partial class OnePassTypesJsonContext : JsonSerializerContext
{
}

internal record struct ResolveSecretPayload()
{
	[JsonPropertyName("secret_reference")]
	public required string SecretReference { get; set; }

	internal static string ToJson(ResolveSecretPayload value) => JsonSerializer.Serialize(value, OnePassTypesJsonContext.Default.ResolveSecretPayload);
	internal static ResolveSecretPayload FromJson(string value) => JsonSerializer.Deserialize(value, OnePassTypesJsonContext.Default.ResolveSecretPayload);
}

internal record struct ResolveSecretsPayload()
{
	[JsonPropertyName("secret_references")]
	public required IReadOnlyList<string> SecretReferences { get; set; }

	internal static string ToJson(ResolveSecretsPayload value) => JsonSerializer.Serialize(value, OnePassTypesJsonContext.Default.ResolveSecretsPayload);
	internal static ResolveSecretsPayload FromJson(string value) => JsonSerializer.Deserialize(value, OnePassTypesJsonContext.Default.ResolveSecretsPayload);
}

internal record struct ValidateSecretReferencePayload()
{
	[JsonPropertyName("secret_reference")]
	public required string SecretReference { get; set; }

	internal static string ToJson(ValidateSecretReferencePayload value) => JsonSerializer.Serialize(value, OnePassTypesJsonContext.Default.ValidateSecretReferencePayload);
	internal static ValidateSecretReferencePayload FromJson(string value) => JsonSerializer.Deserialize(value, OnePassTypesJsonContext.Default.ValidateSecretReferencePayload);
}

internal record struct GetAllItemsPayload()
{
	[JsonPropertyName("vault_id")]
	public required string VaultId { get; set; }

	[JsonPropertyName("item_ids")]
	public required IReadOnlyList<string> ItemIds { get; set; }

	internal static string ToJson(GetAllItemsPayload value) => JsonSerializer.Serialize(value, OnePassTypesJsonContext.Default.GetAllItemsPayload);
	internal static GetAllItemsPayload FromJson(string value) => JsonSerializer.Deserialize(value, OnePassTypesJsonContext.Default.GetAllItemsPayload);
}

internal record struct GetItemsPayload()
{
	[JsonPropertyName("vault_id")]
	public required string VaultId { get; set; }

	[JsonPropertyName("item_id")]
	public required string ItemId { get; set; }

	internal static string ToJson(GetItemsPayload value) => JsonSerializer.Serialize(value, OnePassTypesJsonContext.Default.GetItemsPayload);
	internal static GetItemsPayload FromJson(string value) => JsonSerializer.Deserialize(value, OnePassTypesJsonContext.Default.GetItemsPayload);
}

internal record struct GetItemPayload()
{
	[JsonPropertyName("vault_id")]
	public required string VaultId { get; set; }

	[JsonPropertyName("item_id")]
	public required IReadOnlyList<string> ItemIds { get; set; }

	internal static string ToJson(GetItemPayload value) => JsonSerializer.Serialize(value, OnePassTypesJsonContext.Default.GetItemPayload);
	internal static GetItemPayload FromJson(string value) => JsonSerializer.Deserialize(value, OnePassTypesJsonContext.Default.GetItemPayload);
}

internal record struct ListItemsPayload()
{
	[JsonPropertyName("vault_id")]
	public required string VaultId { get; set; }

	[JsonPropertyName("filters")]
	public required IReadOnlyList<ItemListFilter> Filters { get; set; }

	internal static string ToJson(ListItemsPayload value) => JsonSerializer.Serialize(value, OnePassTypesJsonContext.Default.ListItemsPayload);
	internal static ListItemsPayload FromJson(string value) => JsonSerializer.Deserialize(value, OnePassTypesJsonContext.Default.ListItemsPayload);
}

internal record struct ItemsPutPayload()
{
	[JsonPropertyName("item")]
	public required Item Item { get; set; }

	internal static string ToJson(ItemsPutPayload value) => JsonSerializer.Serialize(value, OnePassTypesJsonContext.Default.ItemsPutPayload);
	internal static ItemsPutPayload FromJson(string value) => JsonSerializer.Deserialize(value, OnePassTypesJsonContext.Default.ItemsPutPayload);
}

internal record struct DeleteItemPayload()
{
	[JsonPropertyName("vault_id")]
	public required string VaultId { get; set; }

	[JsonPropertyName("item_id")]
	public required string ItemId { get; set; }

	internal static string ToJson(DeleteItemPayload value) => JsonSerializer.Serialize(value, OnePassTypesJsonContext.Default.DeleteItemPayload);
	internal static DeleteItemPayload FromJson(string value) => JsonSerializer.Deserialize(value, OnePassTypesJsonContext.Default.DeleteItemPayload);
}

internal record struct DeleteAllItemsPayload()
{
	[JsonPropertyName("vault_id")]
	public required string VaultId { get; set; }

	[JsonPropertyName("item_ids")]
	public required IReadOnlyList<string> ItemIds { get; set; }

	internal static string ToJson(DeleteAllItemsPayload value) => JsonSerializer.Serialize(value, OnePassTypesJsonContext.Default.DeleteAllItemsPayload);
	internal static DeleteAllItemsPayload FromJson(string value) => JsonSerializer.Deserialize(value, OnePassTypesJsonContext.Default.DeleteAllItemsPayload);
}

internal record struct ItemsCreatePayload()
{
	[JsonPropertyName("params")]
	public required ItemCreateParams Params { get; set; }

	internal static string ToJson(ItemsCreatePayload value) => JsonSerializer.Serialize(value, OnePassTypesJsonContext.Default.ItemsCreatePayload);
	internal static ItemsCreatePayload FromJson(string value) => JsonSerializer.Deserialize(value, OnePassTypesJsonContext.Default.ItemsCreatePayload);
}

internal record struct ItemsCreateAllPayload()
{
	[JsonPropertyName("vault_id")]
	public required string VaultId { get; set; }

	[JsonPropertyName("params")]
	public required IReadOnlyList<ItemCreateParams> Params { get; set; }

	internal static string ToJson(ItemsCreateAllPayload value) => JsonSerializer.Serialize(value, OnePassTypesJsonContext.Default.ItemsCreateAllPayload);
	internal static ItemsCreateAllPayload FromJson(string value) => JsonSerializer.Deserialize(value, OnePassTypesJsonContext.Default.ItemsCreateAllPayload);
}

