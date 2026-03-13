using System.Text.Json.Serialization;

namespace Keyden.OnePassword.SDK.Types;

public struct Website()
{
	[JsonPropertyName("url")]
	public required string Url { get; set; }

	[JsonPropertyName("label")]
	public required string Label { get; set; }

	[JsonPropertyName("autofillBehavior")]
	[JsonConverter(typeof(AutofillBehaviorJsonConverter))]
	public required AutofillBehavior AutofillBehavior { get; set; }
}
