using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json.Nodes;

namespace Keyden;

file static class JsonExtensionMethods
{
	public static JsonValue AsJsonValue(this TimeSpan value)
	{
		return JsonValue.Create(value.ToString(null, CultureInfo.InvariantCulture));
	}
	public static JsonValue AsJsonValue(this bool value)
	{
		return JsonValue.Create(value);
	}
	public static JsonArray AsJsonValue(this IEnumerable<string> value)
	{
		return [.. value.Select(x => JsonValue.Create(x))];
	}
	public static bool? GetBool(this JsonNode node)
	{
		if (node.AsValue() is not JsonValue value)
			return null;
		return value.GetValue<bool>();
	}
	public static TimeSpan? GetTimeSpan(this JsonNode node)
	{
		if (node.AsValue() is not JsonValue value)
			return null;
		var str = value.GetValue<string>();
		return TimeSpan.Parse(str, CultureInfo.InvariantCulture);
	}
	public static IReadOnlyList<string>? GetStringList(this JsonNode node)
	{
		if (node.AsArray() is not JsonArray array)
			return null;
		return array
			.Select(x => x?.GetValue<string>())
			.Where(x => !string.IsNullOrWhiteSpace(x))
			.Select(x => x!)
			.ToList();
	}
}

public static class KeyOptionsJson
{
	public static SshKeyOptions ReadOptionsNode(JsonObject obj)
	{
		var @default = new SshKeyOptions();
		return new SshKeyOptions()
		{
			EnableForMachines = obj["EnableForMachines"]?.GetStringList() ?? @default.EnableForMachines,
			RequireAuthorization = obj["RequireAuthorization"]?.GetBool() ?? @default.RequireAuthentication,
			RemainAuthorized = obj["RemainAuthorized"]?.GetBool() ?? @default.RemainAuthorized,
			RemainAuthorizedFor = obj["RemainAuthorizedFor"]?.GetTimeSpan() ?? @default.RemainAuthorizedFor,
			RemainAuthorizedUntilKeyInactivity = obj["RemainAuthorizedUntilKeyInactivity"]?.GetBool() ?? @default.RemainAuthorizedUntilKeyInactivity,
			RemainAuthorizedUntilKeyInactivityFor = obj["RemainAuthorizedUntilKeyInactivityFor"]?.GetTimeSpan() ?? @default.RemainAuthorizedUntilKeyInactivityFor,
			RemainAuthorizedUntilUserInactivity = obj["RemainAuthorizedUntilUserInactivity"]?.GetBool() ?? @default.RemainAuthorizedUntilUserInactivity,
			RemainAuthorizedUntilUserInactivityFor = obj["RemainAuthorizedUntilUserInactivityFor"]?.GetTimeSpan() ?? @default.RemainAuthorizedUntilUserInactivityFor,
			RemainAuthorizedUntilLocked = obj["RemainAuthorizedUntilLocked"]?.GetBool() ?? @default.RemainAuthorizedUntilLocked,
			RequireAuthentication = obj["RequireAuthentication"]?.GetBool() ?? @default.RequireAuthentication,
			RemainAuthenticated = obj["RemainAuthenticated"]?.GetBool() ?? @default.RemainAuthenticated,
			RemainAuthenticatedFor = obj["RemainAuthenticatedFor"]?.GetTimeSpan() ?? @default.RemainAuthenticatedFor,
			RemainAuthenticatedUntilKeyInactivity = obj["RemainAuthenticatedUntilKeyInactivity"]?.GetBool() ?? @default.RemainAuthenticatedUntilKeyInactivity,
			RemainAuthenticatedUntilKeyInactivityFor = obj["RemainAuthenticatedUntilKeyInactivityFor"]?.GetTimeSpan() ?? @default.RemainAuthenticatedUntilKeyInactivityFor,
			RemainAuthenticatedUntilUserInactivity = obj["RemainAuthenticatedUntilUserInactivity"]?.GetBool() ?? @default.RemainAuthenticatedUntilUserInactivity,
			RemainAuthenticatedUntilUserInactivityFor = obj["RemainAuthenticatedUntilUserInactivityFor"]?.GetTimeSpan() ?? @default.RemainAuthenticatedUntilUserInactivityFor,
			RemainAuthenticatedUntilLocked = obj["RemainAuthenticatedUntilLocked"]?.GetBool() ?? @default.RemainAuthenticatedUntilLocked,
		};
	}

	public static void WriteOptionsNode(JsonObject obj, SshKeyOptions opts)
	{
		obj["EnableForMachines"] = opts.EnableForMachines.AsJsonValue();
		obj["RequireAuthorization"] = opts.RequireAuthorization.AsJsonValue();
		obj["RemainAuthorized"] = opts.RemainAuthorized.AsJsonValue();
		obj["RemainAuthorizedFor"] = opts.RemainAuthorizedFor.AsJsonValue();
		obj["RemainAuthorizedUntilKeyInactivity"] = opts.RemainAuthorizedUntilKeyInactivity.AsJsonValue();
		obj["RemainAuthorizedUntilKeyInactivityFor"] = opts.RemainAuthorizedUntilKeyInactivityFor.AsJsonValue();
		obj["RemainAuthorizedUntilUserInactivity"] = opts.RemainAuthorizedUntilUserInactivity.AsJsonValue();
		obj["RemainAuthorizedUntilUserInactivityFor"] = opts.RemainAuthorizedUntilUserInactivityFor.AsJsonValue();
		obj["RemainAuthorizedUntilLocked"] = opts.RemainAuthorizedUntilLocked.AsJsonValue();
		obj["RequireAuthentication"] = opts.RequireAuthentication.AsJsonValue();
		obj["RemainAuthenticated"] = opts.RemainAuthenticated.AsJsonValue();
		obj["RemainAuthenticatedFor"] = opts.RemainAuthenticatedFor.AsJsonValue();
		obj["RemainAuthenticatedUntilKeyInactivity"] = opts.RemainAuthenticatedUntilKeyInactivity.AsJsonValue();
		obj["RemainAuthenticatedUntilKeyInactivityFor"] = opts.RemainAuthenticatedUntilKeyInactivityFor.AsJsonValue();
		obj["RemainAuthenticatedUntilUserInactivity"] = opts.RemainAuthenticatedUntilUserInactivity.AsJsonValue();
		obj["RemainAuthenticatedUntilUserInactivityFor"] = opts.RemainAuthenticatedUntilUserInactivityFor.AsJsonValue();
		obj["RemainAuthenticatedUntilLocked"] = opts.RemainAuthenticatedUntilLocked.AsJsonValue();
	}
}
