using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;

using Keyden.OnePassword.SDK.Types;

namespace Keyden.OnePassword.SDK;

internal unsafe static partial class OnePassSdkInterop
{
	[JsonSerializable(typeof(PayloadMessage))]
	[JsonSerializable(typeof(PayloadResponse))]
	[JsonSerializable(typeof(PayloadTypedError))]
	[JsonSerializable(typeof(InvocationPayload))]
	internal partial class InteropJsonContext : JsonSerializerContext
	{
	}

	[JsonConverter(typeof(PayloadKindJsonConverter))]
	public enum PayloadKind
	{
		None,
		InitClient,
		Invoke,
		ReleaseClient,
	}

	private sealed class PayloadKindJsonConverter : JsonConverter<PayloadKind>
	{
		public override PayloadKind Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (reader.TokenType != JsonTokenType.String)
				throw new JsonException($"Unexpected token parsing {nameof(AutofillBehavior)}. Expected String, got {reader.TokenType}.");

			return reader.GetString() switch
			{
				"none" => PayloadKind.None,
				"init_client" => PayloadKind.InitClient,
				"invoke" => PayloadKind.Invoke,
				"release_client" => PayloadKind.ReleaseClient,
				_ => throw new NotImplementedException(),
			};
		}

		public override void Write(Utf8JsonWriter writer, PayloadKind value, JsonSerializerOptions options)
		{
			var valueStr = value switch
			{
				PayloadKind.None => "none",
				PayloadKind.InitClient => "init_client",
				PayloadKind.Invoke => "invoke",
				PayloadKind.ReleaseClient => "release_client",
				_ => throw new NotImplementedException(),
			};
			writer.WriteStringValue(valueStr);
		}
	}

	public record struct PayloadMessage()
	{
		[JsonPropertyName("kind")]
		public PayloadKind Kind { get; set; } = PayloadKind.None;

		[JsonPropertyName("account_name")]
		public required string AccountName { get; set; }

		[JsonPropertyName("payload")]
		[JsonConverter(typeof(Base64JsonConverter))]
		public required string Payload { get; set; }
	}

	public record struct PayloadResponse()
	{
		[JsonPropertyName("success")]
		public required bool Success { get; set; } = false;

		[JsonPropertyName("payload")]
		[JsonConverter(typeof(ByteArrayJsonConverter))]
		public required string Payload { get; set; }
	}

	public record struct PayloadTypedError
	{
		public PayloadTypedError() { }
		[JsonPropertyName("name")]
		public string ErrorName { get; set; } = string.Empty;
		[JsonPropertyName("message")]
		public string Message { get; set; } = string.Empty;
	}

	[DoesNotReturn]
	public static void ThrowTypedException(string payload)
	{
		PayloadTypedError typedError;
		try
		{
			typedError = JsonSerializer.Deserialize(payload, InteropJsonContext.Default.PayloadTypedError);
		}
		catch
		{
			throw new OnePassSdkException(payload);
		}

		throw typedError switch
		{
			{ ErrorName: "DesktopSessionExpired" } => new OnePassSdkDesktopSessionExpiredException(payload),
			{ ErrorName: "RateLimitExceeded" } => new OnePassSdkRateLimitExceededException(payload),
			{ Message: not "" or null } => new OnePassSdkException(typedError.Message),
			_ => new OnePassSdkException(payload),
		};
	}

	public static PayloadResponse CallSharedLibrary(PayloadMessage payload)
	{
		var json = JsonSerializer.Serialize(payload, InteropJsonContext.Default.PayloadMessage);
		var returnCode = OnePassSdkNative.SendMessage(json, out var responseStr);

		if (returnCode != 0)
		{
			bool isOsx = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
			throw returnCode switch
			{
				-3 when isOsx => new OnePassSdkChannelClosedException(),
				-7 when isOsx => new OnePassSdkChannelConnectionDroppedException(),
				-2 when !isOsx => new OnePassSdkChannelClosedException(),
				-5 when !isOsx => new OnePassSdkChannelConnectionDroppedException(),
				_ => new OnePassSdkInternalException(returnCode),
			};
		}

		var response = JsonSerializer.Deserialize(responseStr, InteropJsonContext.Default.PayloadResponse);
		if (!response.Success)
			ThrowTypedException(response.Payload);

		return response;
	}

	public static string Invoke(string accountName, InvocationPayload payload)
	{
		var json = JsonSerializer.Serialize(payload, InteropJsonContext.Default.InvocationPayload);

		var ret = CallSharedLibrary(new()
		{
			Kind = PayloadKind.Invoke,
			AccountName = accountName,
			Payload = json,
		});

		return ret.Payload;
	}

	public record struct InvocationPayload()
	{
		[JsonPropertyName("invocation")]
		public required InvocationObject Invocation { get; set; }

		public record struct InvocationObject()
		{
			[JsonPropertyName("clientId")]
			public required int ClientId { get; set; }

			[JsonPropertyName("parameters")]
			public required ParametersObject Parameters { get; set; }

			public record struct ParametersObject()
			{
				[JsonPropertyName("name")]
				public required string Name { get; set; } = "";

				[JsonPropertyName("parameters")]
				[JsonConverter(typeof(RawJsonConverter))]
				public required string Parameters { get; set; }
			}
		}
	}
}
