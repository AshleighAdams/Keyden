using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Keyden.OnePassword.SDK.Types;

public struct ItemsGetAllResponse()
{
	[JsonPropertyName("individualResponses")]
	public required IReadOnlyList<Response<Item, ItemErrorResponse>> IndividualResponses { get; set; }

	public static string ToJson(ItemsGetAllResponse value) => JsonSerializer.Serialize(value, OnePassTypesJsonContext.Default.ItemsGetAllResponse);
	public static ItemsGetAllResponse FromJson(string value) => JsonSerializer.Deserialize(value, OnePassTypesJsonContext.Default.ItemsGetAllResponse);
}
