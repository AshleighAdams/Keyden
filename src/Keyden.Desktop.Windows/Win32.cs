using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Keyden.Desktop.Windows;

[SupportedOSPlatform("windows")]
public static partial class Win32
{
	[LibraryImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static partial bool IsImmersiveProcess(nint process);

	[LibraryImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static partial bool GetWindowRect(nint handle, out Rect rect);

	[StructLayout(LayoutKind.Sequential)]
	public struct Rect
	{
		public int Left;
		public int Top;
		public int Right;
		public int Bottom;
	}

	[LibraryImport("user32.dll")]
	public static partial nint GetForegroundWindow();

	[LibraryImport("user32.dll", EntryPoint = "GetLastInputInfo")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static partial bool GetLastInputInfo(ref LastInputInfo value);

	[StructLayout(LayoutKind.Sequential)]
	public struct LastInputInfo
	{
		public static readonly int SizeOf = Marshal.SizeOf<LastInputInfo>();

		[MarshalAs(UnmanagedType.U4)]
		public int Size;
		[MarshalAs(UnmanagedType.U4)]
		public int Time;
	}

	[LibraryImport("kernel32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static partial bool GetNamedPipeClientProcessId(nint pipe, out int clientProcessId);


	[LibraryImport("ntdll.dll")]
	public static partial int NtQueryInformationProcess(IntPtr processHandle, int processInformationClass, ref ProcessBasicInformation processInformation, int processInformationLength, out int returnLength);

	[StructLayout(LayoutKind.Sequential)]
	public struct ProcessBasicInformation
	{
		public IntPtr Reserved1;
		public IntPtr PebBaseAddress;
		public IntPtr Reserved2_0;
		public IntPtr Reserved2_1;
		public IntPtr UniqueProcessId;
		public IntPtr InheritedFromUniqueProcessId;
	}
}
