using System.Text.Json.Serialization;

namespace Keyden.OnePassword.SDK.Types;

public struct FileAttributes()
{
	[JsonPropertyName("name")]
	public required string Name { get; set; }

	[JsonPropertyName("id")]
	public required string Id { get; set; }

	[JsonPropertyName("size")]
	public required int Size { get; set; }
}
