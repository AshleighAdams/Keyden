using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Runtime.InteropServices;

namespace Keyden.OnePassword.SDK.Types;

using CpuArchitecture = Architecture;

public record class OnePassSdkConfig
{
	[JsonPropertyName("programmingLanguage")]
	public string ProgrammingLanguage { get; init; } = "C# (Keyden)";

	[JsonPropertyName("sdkVersion")]
	public string SdkVersion { get; init; } = "0040003";

	[JsonPropertyName("integrationName")]
	public string IntegrationName { get; init; } = "Unknown";

	[JsonPropertyName("integrationVersion")]
	public string IntegrationVersion { get; init; } = "Unknown";

	[JsonPropertyName("requestLibraryName")]
	public string RequestLibraryName { get; init; } = "none";

	[JsonPropertyName("requestLibraryVersion")]
	public string RequestLibraryVersion { get; init; } = "0.0.0";

	[JsonPropertyName("os")]
	public string OS { get; init; } = Environment.OSVersion.Platform switch
	{
		PlatformID.Win32S => "Windows",
		PlatformID.Win32Windows => "Windows",
		PlatformID.Win32NT => "Windows",
		PlatformID.WinCE => "Windows",
		PlatformID.Unix => "Linux/Unix",
		PlatformID.Xbox => "Xbox",
		PlatformID.MacOSX => "MacOS",
		PlatformID.Other => "Unknown",
		_ => "Unknown",
	};

	[JsonPropertyName("osVersion")]
	public string OSVersion { get; init; } = Environment.OSVersion.Version.ToString();

	[JsonPropertyName("architecture")]
	public string Architecture { get; init; } = RuntimeInformation.ProcessArchitecture switch
	{
		CpuArchitecture.X86 => "x86",
		CpuArchitecture.X64 => "x64",
		CpuArchitecture.Arm => "arm",
		CpuArchitecture.Arm64 => "arm64",
		CpuArchitecture.Wasm => "wasm",
		CpuArchitecture.S390x => "s390x",
		CpuArchitecture.LoongArch64 => "loongarch64",
		CpuArchitecture.Armv6 => "armv6",
		CpuArchitecture.Ppc64le => "ppc64le",
		CpuArchitecture.RiscV64 => "riscv64",
		_ => throw new NotImplementedException(),
	};

	internal static string ToJson(OnePassSdkConfig value) => JsonSerializer.Serialize(value, OnePassTypesJsonContext.Default.OnePassSdkConfig);
	internal static OnePassSdkConfig? FromJson(string value) => JsonSerializer.Deserialize(value, OnePassTypesJsonContext.Default.OnePassSdkConfig);
}
