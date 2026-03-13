using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Keyden.OnePassword.SDK.Types;

[JsonConverter(typeof(AutofillBehaviorJsonConverter))]
public enum AutofillBehavior
{
	AnywhereOnWebsite,
	ExactDomain,
	Never,
	Unknown,
}

public sealed class AutofillBehaviorJsonConverter : JsonConverter<AutofillBehavior>
{
	public override AutofillBehavior Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType != JsonTokenType.String)
			throw new JsonException($"Unexpected token parsing {nameof(AutofillBehavior)}. Expected String, got {reader.TokenType}.");

		return reader.GetString() switch
		{
			"AnywhereOnWebsite" => AutofillBehavior.AnywhereOnWebsite,
			"ExactDomain" => AutofillBehavior.ExactDomain,
			"Never" => AutofillBehavior.Never,
			_ => AutofillBehavior.Unknown,
		};
	}

	public override void Write(Utf8JsonWriter writer, AutofillBehavior value, JsonSerializerOptions options)
	{
		var valueStr = value switch
		{
			AutofillBehavior.AnywhereOnWebsite => "AnywhereOnWebsite",
			AutofillBehavior.ExactDomain => "ExactDomain",
			AutofillBehavior.Never => "Never",
			_ => throw new NotImplementedException(),
		};
		writer.WriteStringValue(valueStr);
	}
}
