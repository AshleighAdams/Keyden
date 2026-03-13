using System.Text.Json.Serialization;

namespace Keyden.OnePassword.SDK.Types;

public struct ItemFieldDetails()
{
	[JsonPropertyName("type")]
	public required string Type { get; set; } // Otp/SshKey/Address

	[JsonPropertyName("content")]
	[JsonConverter(typeof(RawJsonConverter))]
	public required string Content { get; set; } // https://github.com/1Password/onepassword-sdk-python/blob/main/src/onepassword/types.py#L335
}
