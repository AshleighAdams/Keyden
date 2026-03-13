using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Keyden.OnePassword.SDK.Types;

[JsonConverter(typeof(ItemCategoryJsonConverter))]
public enum ItemCategory
{
	Login,
	SecureNote,
	CreditCard,
	CryptoWallet,
	Identity,
	Password,
	Document,
	ApiCredentials,
	BankAccount,
	Database,
	DriverLicense,
	Email,
	MedicalRecord,
	Membership,
	OutdoorLicense,
	Passport,
	Rewards,
	Router,
	Server,
	SshKey,
	SocialSecurityNumber,
	SoftwareLicense,
	Person,
	Unsupported,
	Unknown,
}

public sealed class ItemCategoryJsonConverter : JsonConverter<ItemCategory>
{
	public override ItemCategory Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType != JsonTokenType.String)
			throw new JsonException($"Unexpected token parsing {nameof(ItemCategory)}. Expected String, got {reader.TokenType}.");

		return reader.GetString() switch
		{
			"Login" => ItemCategory.Login,
			"SecureNote" => ItemCategory.SecureNote,
			"CreditCard" => ItemCategory.CreditCard,
			"CryptoWallet" => ItemCategory.CryptoWallet,
			"Identity" => ItemCategory.Identity,
			"Password" => ItemCategory.Password,
			"Document" => ItemCategory.Document,
			"ApiCredentials" => ItemCategory.ApiCredentials,
			"BankAccount" => ItemCategory.BankAccount,
			"Database" => ItemCategory.Database,
			"DriverLicense" => ItemCategory.DriverLicense,
			"Email" => ItemCategory.Email,
			"MedicalRecord" => ItemCategory.MedicalRecord,
			"Membership" => ItemCategory.Membership,
			"OutdoorLicense" => ItemCategory.OutdoorLicense,
			"Passport" => ItemCategory.Passport,
			"Rewards" => ItemCategory.Rewards,
			"Router" => ItemCategory.Router,
			"Server" => ItemCategory.Server,
			"SshKey" => ItemCategory.SshKey,
			"SocialSecurityNumber" => ItemCategory.SocialSecurityNumber,
			"SoftwareLicense" => ItemCategory.SoftwareLicense,
			"Person" => ItemCategory.Person,
			"Unsupported" => ItemCategory.Unsupported,
			_ => ItemCategory.Unknown,
		};
	}

	public override void Write(Utf8JsonWriter writer, ItemCategory value, JsonSerializerOptions options)
	{
		var valueStr = value switch
		{
			ItemCategory.Login => "Login",
			ItemCategory.SecureNote => "SecureNote",
			ItemCategory.CreditCard => "CreditCard",
			ItemCategory.CryptoWallet => "CryptoWallet",
			ItemCategory.Identity => "Identity",
			ItemCategory.Password => "Password",
			ItemCategory.Document => "Document",
			ItemCategory.ApiCredentials => "ApiCredentials",
			ItemCategory.BankAccount => "BankAccount",
			ItemCategory.Database => "Database",
			ItemCategory.DriverLicense => "DriverLicense",
			ItemCategory.Email => "Email",
			ItemCategory.MedicalRecord => "MedicalRecord",
			ItemCategory.Membership => "Membership",
			ItemCategory.OutdoorLicense => "OutdoorLicense",
			ItemCategory.Passport => "Passport",
			ItemCategory.Rewards => "Rewards",
			ItemCategory.Router => "Router",
			ItemCategory.Server => "Server",
			ItemCategory.SshKey => "SshKey",
			ItemCategory.SocialSecurityNumber => "SocialSecurityNumber",
			ItemCategory.SoftwareLicense => "SoftwareLicense",
			ItemCategory.Person => "Person",
			ItemCategory.Unsupported => "Unsupported",
			_ => throw new NotImplementedException(),
		};
		writer.WriteStringValue(valueStr);
	}
}
