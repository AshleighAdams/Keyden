using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Keyden.OnePassword.SDK.Types;

public struct ItemsDeleteAllResponse()
{
	[JsonPropertyName("individualResponses")]
	public required IReadOnlyList<Response<None, ItemUpdateFailureReason>> IndividualResponses { get; set; }

	public static string ToJson(ItemsDeleteAllResponse value) => JsonSerializer.Serialize(value, OnePassTypesJsonContext.Default.ItemsDeleteAllResponse);
	public static ItemsDeleteAllResponse FromJson(string value) => JsonSerializer.Deserialize(value, OnePassTypesJsonContext.Default.ItemsDeleteAllResponse);
}
