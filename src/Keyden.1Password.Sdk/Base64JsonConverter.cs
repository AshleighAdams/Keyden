using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Keyden.OnePassword.SDK;

internal class Base64JsonConverter : JsonConverter<string>
{
	public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		return OnePassSdkNative.Utf8.GetString(reader.GetBytesFromBase64());
	}

	public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
	{
		writer.WriteBase64StringValue(OnePassSdkNative.Utf8.GetBytes(value));
	}
}
