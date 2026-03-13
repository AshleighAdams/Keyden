using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Keyden.OnePassword.SDK.Types;

public struct ItemCreateParams()
{
	[JsonPropertyName("category")]
	public required ItemCategory Category { get; set; }

	[JsonPropertyName("vaultId")]
	public required string VaultId { get; set; }

	[JsonPropertyName("title")]
	public required string Title { get; set; }

	[JsonPropertyName("fields")]
	public IReadOnlyList<ItemField>? Fields { get; set; } = null;

	[JsonPropertyName("sections")]
	public IReadOnlyList<ItemSection>? Sections { get; set; } = null;

	[JsonPropertyName("notes")]
	public string? Notes { get; set; } = null;

	[JsonPropertyName("tags")]
	public IReadOnlyList<string>? Tags { get; set; } = null;

	[JsonPropertyName("websites")]
	public IReadOnlyList<Website>? Websites { get; set; } = null;

	[JsonPropertyName("files")]
	public IReadOnlyList<FileCreateParams>? Files { get; set; } = null;

	[JsonPropertyName("document")]
	public DocumentCreateParams? Document { get; set; } = null;

	public static string ToJson(ItemCreateParams value) => JsonSerializer.Serialize(value, OnePassTypesJsonContext.Default.ItemCreateParams);
	public static ItemCreateParams FromJson(string value) => JsonSerializer.Deserialize(value, OnePassTypesJsonContext.Default.ItemCreateParams);

	public static string ToJson(IReadOnlyList<ItemCreateParams> value) => JsonSerializer.Serialize(value, OnePassTypesJsonContext.Default.IReadOnlyListItemCreateParams);

}
