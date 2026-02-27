using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Keyden.OnePassword.SDK.Types;

[JsonConverter(typeof(ItemStateJsonConverter))]
public enum ItemState
{
	Active,
	Archived,
	Unknown,
}

public sealed class ItemStateJsonConverter : JsonConverter<ItemState>
{
	public override ItemState Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType != JsonTokenType.String)
			throw new JsonException($"Unexpected token parsing {nameof(ItemState)}. Expected String, got {reader.TokenType}.");

		return reader.GetString() switch
		{
			"active" => ItemState.Active,
			"archived" => ItemState.Archived,
			_ => ItemState.Unknown,
		};
	}

	public override void Write(Utf8JsonWriter writer, ItemState value, JsonSerializerOptions options)
	{
		var valueStr = value switch
		{
			ItemState.Active => "active",
			ItemState.Archived => "archived",
			_ => throw new NotImplementedException(),
		};
		writer.WriteStringValue(valueStr);
	}
}
