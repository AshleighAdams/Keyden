using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Keyden.OnePassword.SDK;

internal class ByteArrayJsonConverter : JsonConverter<string>
{
	public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		using var ms = new MemoryStream();

		if (reader.TokenType != JsonTokenType.StartArray)
			throw new JsonException($"Expected start array, got {reader.TokenType}", null, null, reader.TokenStartIndex);
		
		while (reader.Read() && reader.TokenType == JsonTokenType.Number)
		{
			if (!reader.TryGetByte(out byte x))
				throw new JsonException($"Expected number between [0..255]", null, null, reader.TokenStartIndex);
			ms.WriteByte(x);
		}
		
		if (reader.TokenType != JsonTokenType.EndArray)
			throw new JsonException($"Expected end array, got {reader.TokenType}", null, null, reader.TokenStartIndex);

		ms.Flush();
		return OnePassSdkNative.Utf8.GetString(ms.ToArray());
	}

	public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
	{
		var bytes = OnePassSdkNative.Utf8.GetBytes(value);

		writer.WriteStartArray();
		foreach (byte x in bytes)
			writer.WriteNumberValue(x);
		writer.WriteEndArray();
	}
}
