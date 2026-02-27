using System.Text.Json;

namespace Keyden.OnePassword.SDK.Types;

public record struct GeneratePasswordResponse()
{

	internal static string ToJson(GeneratePasswordResponse value) => JsonSerializer.Serialize(value, OnePassTypesJsonContext.Default.GeneratePasswordResponse);
	internal static GeneratePasswordResponse FromJson(string value) => JsonSerializer.Deserialize(value, OnePassTypesJsonContext.Default.GeneratePasswordResponse);
}
