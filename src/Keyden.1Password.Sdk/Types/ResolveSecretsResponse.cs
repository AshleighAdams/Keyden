using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Keyden.OnePassword.SDK.Types;

public record struct ResolveSecretsResponse()
{
	public struct ResponseObject()
	{
		[JsonPropertyName("content")]
		public ContentObject? Content { get; set; }
		public struct ContentObject()
		{
			[JsonPropertyName("secret")]
			public string Secret { get; set; } = string.Empty;

			[JsonPropertyName("itemId")]
			public string ItemId { get; set; } = string.Empty;

			[JsonPropertyName("vaultId")]
			public string VaultId { get; set; } = string.Empty;
		}

		[JsonPropertyName("error")]
		public ErrorObject? Error { get; set; }
		public struct ErrorObject()
		{
			[JsonPropertyName("type")]
			public string Type { get; set; } = string.Empty;
		}
	}

	[JsonPropertyName("individualResponses")]
	public required IReadOnlyDictionary<string, ResponseObject> IndividualResponses { get; set; }

	internal static string ToJson(ResolveSecretsResponse value) => JsonSerializer.Serialize(value, OnePassTypesJsonContext.Default.ResolveSecretsResponse);
	internal static ResolveSecretsResponse FromJson(string value) => JsonSerializer.Deserialize(value, OnePassTypesJsonContext.Default.ResolveSecretsResponse);
}
