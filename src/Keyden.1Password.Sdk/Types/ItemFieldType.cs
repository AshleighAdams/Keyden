using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Keyden.OnePassword.SDK.Types;

public enum ItemFieldType
{
	Text,
	Concealed,
	CreditCardType,
	CreditCardNumber,
	Phone,
	Url,
	Totp,
	Email,
	Reference,
	SshKey,
	Menu,
	MonthYear,
	Address,
	Date,
	Unsupported,
	Unknown,
}

public sealed class ItemFieldTypeJsonConverter : JsonConverter<ItemFieldType>
{
	public override ItemFieldType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType != JsonTokenType.String)
			throw new JsonException($"Unexpected token parsing {nameof(ItemFieldType)}. Expected String, got {reader.TokenType}.");

		return reader.GetString() switch
		{
			"Text" => ItemFieldType.Text,
			"Concealed" => ItemFieldType.Concealed,
			"CreditCardType" => ItemFieldType.CreditCardType,
			"CreditCardNumber" => ItemFieldType.CreditCardNumber,
			"Phone" => ItemFieldType.Phone,
			"Url" => ItemFieldType.Url,
			"Totp" => ItemFieldType.Totp,
			"Email" => ItemFieldType.Email,
			"Reference" => ItemFieldType.Reference,
			"SshKey" => ItemFieldType.SshKey,
			"Menu" => ItemFieldType.Menu,
			"MonthYear" => ItemFieldType.MonthYear,
			"Address" => ItemFieldType.Address,
			"Date" => ItemFieldType.Date,
			"Unsupported" => ItemFieldType.Unsupported,
			_ => ItemFieldType.Unknown,
		};
	}

	public override void Write(Utf8JsonWriter writer, ItemFieldType value, JsonSerializerOptions options)
	{
		var valueStr = value switch
		{
			ItemFieldType.Text => "Text",
			ItemFieldType.Concealed => "Concealed",
			ItemFieldType.CreditCardType => "CreditCardType",
			ItemFieldType.CreditCardNumber => "CreditCardNumber",
			ItemFieldType.Phone => "Phone",
			ItemFieldType.Url => "Url",
			ItemFieldType.Totp => "Totp",
			ItemFieldType.Email => "Email",
			ItemFieldType.Reference => "Reference",
			ItemFieldType.SshKey => "SshKey",
			ItemFieldType.Menu => "Menu",
			ItemFieldType.MonthYear => "MonthYear",
			ItemFieldType.Address => "Address",
			ItemFieldType.Date => "Date",
			ItemFieldType.Unsupported => "Unsupported",
			_ => throw new NotImplementedException(),
		};
		writer.WriteStringValue(valueStr);
	}
}
