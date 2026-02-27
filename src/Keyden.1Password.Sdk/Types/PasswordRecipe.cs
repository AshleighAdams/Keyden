using System.Text.Json;

namespace Keyden.OnePassword.SDK.Types;

public record struct PasswordRecipe()
{

	internal static string ToJson(PasswordRecipe value) => JsonSerializer.Serialize(value, OnePassTypesJsonContext.Default.PasswordRecipe);
	internal static PasswordRecipe FromJson(string value) => JsonSerializer.Deserialize(value, OnePassTypesJsonContext.Default.PasswordRecipe);
}
