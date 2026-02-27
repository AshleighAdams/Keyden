using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Keyden.OnePassword.SDK.Types;

public record struct Item()
{
	[JsonPropertyName("id")]
	public required string Id { get; set; }

	[JsonPropertyName("title")]
	public required string Title { get; set; }

	[JsonPropertyName("category")]
	[JsonConverter(typeof(ItemCategoryJsonConverter))]
	public required ItemCategory Category { get; set; }

	[JsonPropertyName("vaultId")]
	public required string VaultId { get; set; }

	[JsonPropertyName("fields")]
	public required IReadOnlyList<ItemField> Fields { get; set; }

	[JsonPropertyName("sections")]
	public required IReadOnlyList<ItemSection> Sections { get; set; }

	[JsonPropertyName("notes")]
	public required string Notes { get; set; }

	[JsonPropertyName("tags")]
	public required IReadOnlyList<string> Tags { get; set; }

	[JsonPropertyName("websites")]
	public required IReadOnlyList<Website> Websites { get; set; }

	[JsonPropertyName("version")]
	public required int Version { get; set; }

	[JsonPropertyName("files")]
	public required IReadOnlyList<ItemFile> Files { get; set; }

	[JsonPropertyName("document")]
	public FileAttributes? Document { get; set; }

	[JsonPropertyName("createdAt")]
	public DateTime CreatedAt { get; set; }

	[JsonPropertyName("updatedAt")]
	public DateTime UpdatedAt { get; set; }

	public static string ToJson(Item value) => JsonSerializer.Serialize(value, OnePassTypesJsonContext.Default.Item);
	public static Item FromJson(string value) => JsonSerializer.Deserialize(value, OnePassTypesJsonContext.Default.Item);
}
