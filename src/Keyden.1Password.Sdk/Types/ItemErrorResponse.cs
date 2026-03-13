using System.Text.Json.Serialization;

namespace Keyden.OnePassword.SDK.Types;

public struct ItemErrorResponse
{
	[JsonPropertyName("type")]
	public string Type { get; set; } // internal/itemNotFound
	[JsonPropertyName("message")]
	public string? Message { get; set; }
}
