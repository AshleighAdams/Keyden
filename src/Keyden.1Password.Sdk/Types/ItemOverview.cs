using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Keyden.OnePassword.SDK.Types;

public struct ItemOverview()
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

	[JsonPropertyName("websites")]
	public required IReadOnlyList<Website> Websites { get; set; }

	[JsonPropertyName("tags")]
	public required IReadOnlyList<string> Tags { get; set; }

	[JsonPropertyName("createdAt")]
	public required DateTime CreatedAt { get; set; }

	[JsonPropertyName("updatedAt")]
	public required DateTime UpdatedAt { get; set; }

	[JsonPropertyName("state")]
	[JsonConverter(typeof(ItemStateJsonConverter))]
	public required ItemState State { get; set; }
}

public struct ItemListFilter()
{
	// TODO:
}
