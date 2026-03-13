using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Keyden.OnePassword.SDK.Types;

public struct ItemsUpdateAllResponse()
{
	[JsonPropertyName("individualResponses")]
	public required IReadOnlyList<Response<Item, ItemUpdateFailureReason>> IndividualResponses { get; set; }

	public static string ToJson(ItemsUpdateAllResponse value) => JsonSerializer.Serialize(value, OnePassTypesJsonContext.Default.ItemsUpdateAllResponse);
	public static ItemsUpdateAllResponse FromJson(string value) => JsonSerializer.Deserialize(value, OnePassTypesJsonContext.Default.ItemsUpdateAllResponse);
}
