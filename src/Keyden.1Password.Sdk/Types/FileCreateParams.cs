using System.Text.Json.Serialization;

namespace Keyden.OnePassword.SDK.Types;

public struct FileCreateParams()
{
	[JsonPropertyName("name")]
	public required string Name { get; set; }

	[JsonPropertyName("content")]
	public required byte[] Content { get; set; }

	[JsonPropertyName("sectionId")]
	public required string SectionId { get; set; }

	[JsonPropertyName("fieldId")]
	public required string FieldId { get; set; }
}
