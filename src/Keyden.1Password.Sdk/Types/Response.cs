using System.Text.Json.Serialization;

namespace Keyden.OnePassword.SDK.Types;

public struct Response<TValue, TError>
	where TValue : struct
	where TError : struct
{
	[JsonPropertyName("content")]
	public TValue? Content { get; set; }
	[JsonPropertyName("error")]
	public TError? Error { get; set; }
}
