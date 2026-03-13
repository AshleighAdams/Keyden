using System.Text.Json.Serialization;

namespace Keyden.OnePassword.SDK.Types;

public struct DocumentCreateParams()
{
	[JsonPropertyName("name")]
	public required string Name { get; set; }

	[JsonPropertyName("content")]
	public required byte[] Content { get; set; }
}
