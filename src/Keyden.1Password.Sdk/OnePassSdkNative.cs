using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Keyden.OnePassword.SDK;

// using the python SDK as a reference
// https://github.com/1Password/onepassword-sdk-python/blob/main/src/onepassword/desktop_core.py

internal unsafe static partial class OnePassSdkNative
{
	public const string LibSdkIpcClient = "op_sdk_ipc_client";

	[LibraryImport(LibSdkIpcClient, EntryPoint = "op_sdk_ipc_send_message", StringMarshalling = StringMarshalling.Utf8)]
	[UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
	public static partial int SendMessage(byte* msg, nuint msgLen, out byte* response, out nuint responseLen, out nuint responseCap);

	[LibraryImport(LibSdkIpcClient, EntryPoint = "op_sdk_ipc_free_response", StringMarshalling = StringMarshalling.Utf8)]
	[UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
	public static partial void FreeResponse(byte* msg, nuint length, nuint capacity);

	internal static readonly UTF8Encoding Utf8 = new(false);
	public static int SendMessage(string message, out string response)
	{
		var messageUtf8 = Utf8.GetBytes(message);
		var messageUtf8Len = (nuint)messageUtf8.Length;

		byte* responsePtr = null;
		nuint responseLen = nuint.Zero;
		nuint responseCap = nuint.Zero;
		int retCode;

		fixed (byte* messageUtf8Ptr = messageUtf8)
		{
			retCode = SendMessage(messageUtf8Ptr, messageUtf8Len, out responsePtr, out responseLen, out responseCap);
		}

		if ((nuint)responsePtr == nuint.Zero)
		{
			response = string.Empty;
		}
		else
		{
			response = Utf8.GetString(responsePtr, checked((int)responseLen));
			FreeResponse(responsePtr, responseLen, responseCap);
		}

		return retCode;
	}

	static OnePassSdkNative()
	{
		NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), DllImportResolver);
	}

	private static nint DllImportResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
	{
		if (libraryName != LibSdkIpcClient)
			return nint.Zero;

		string[] paths = [];
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			paths = [
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "1password/op_sdk_ipc_client.dll"),
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "1password/app/8/op_sdk_ipc_client.dll"),
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "1password/app/8/op_sdk_ipc_client.dll"),
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "1password/app/8/op_sdk_ipc_client.dll"),
			];
		}
		else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
		{
			paths = [
				Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Applications/1Password.app/Contents/Frameworks/libop_sdk_ipc_client.dylib"),
				Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "1Password.app/Contents/Frameworks/libop_sdk_ipc_client.dylib"),
			];
		}
		else // assume linux/unix
		{
			paths = [
				"/usr/bin/1password/libop_sdk_ipc_client.so",
				"/opt/1Password/libop_sdk_ipc_client.so",
				"/snap/bin/1password/libop_sdk_ipc_client.so",
			];
		}

		foreach (var path in paths)
		{
			if (File.Exists(path))
				return NativeLibrary.Load(path, assembly, null);
		}

		return nint.Zero;
	}
}
