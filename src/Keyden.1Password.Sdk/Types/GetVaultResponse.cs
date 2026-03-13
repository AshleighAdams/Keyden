using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Keyden.OnePassword.SDK.Types;

public record struct GetVaultResponse()
{
	[JsonPropertyName("id")]
	public required string Id { get; set; }

	[JsonPropertyName("title")]
	public required string Title { get; set; }

	[JsonPropertyName("description")]
	public required string Description { get; set; }

	[JsonPropertyName("vaultType")]
	public required string VaultType { get; set; }

	[JsonPropertyName("activeItemCount")]
	public required int ActiveItemCount { get; set; }

	[JsonPropertyName("contentVersion")]
	public required int ContentVersion { get; set; }

	[JsonPropertyName("attributeVersion")]
	public required int AttributeVersion { get; set; }

	[JsonPropertyName("createdAt")]
	public required DateTime CreatedAt { get; set; }

	[JsonPropertyName("updatedAt")]
	public required DateTime UpdatedAt { get; set; }

	internal static string ToJson(GetVaultResponse value) => JsonSerializer.Serialize(value, OnePassTypesJsonContext.Default.GetVaultResponse);
	internal static GetVaultResponse FromJson(string value) => JsonSerializer.Deserialize(value, OnePassTypesJsonContext.Default.GetVaultResponse);
}
