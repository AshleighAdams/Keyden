using System.Text.Json.Serialization;

namespace Keyden.OnePassword.SDK.Types;

public struct ItemField()
{
	[JsonPropertyName("id")]
	public required string Id { get; set; }

	[JsonPropertyName("title")]
	public required string Title { get; set; }

	[JsonPropertyName("sectionId")]
	public string? SectionId { get; set; }

	[JsonPropertyName("fieldType")]
	[JsonConverter(typeof(ItemFieldTypeJsonConverter))]
	public required ItemFieldType FieldType { get; set; }

	[JsonPropertyName("value")]
	public required string Value { get; set; }

	[JsonPropertyName("details")]
	public ItemFieldDetails? Details { get; set; }
}
