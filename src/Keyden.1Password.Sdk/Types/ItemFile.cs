using System.Text.Json.Serialization;

namespace Keyden.OnePassword.SDK.Types;

public struct ItemFile()
{
	[JsonPropertyName("attributes")]
	public required FileAttributes Attributes { get; set; }

	[JsonPropertyName("sectionId")]
	public required string SectionId { get; set; }

	[JsonPropertyName("fieldId")]
	public required string FieldId { get; set; }
}
