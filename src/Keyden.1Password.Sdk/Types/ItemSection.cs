using System.Text.Json.Serialization;

namespace Keyden.OnePassword.SDK.Types;

public struct ItemSection()
{
	[JsonPropertyName("id")]
	public required string Id { get; set; }

	[JsonPropertyName("title")]
	public required string Title {get; set; }
}
