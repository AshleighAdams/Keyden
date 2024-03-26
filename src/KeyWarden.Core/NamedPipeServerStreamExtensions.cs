using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Pipes;
using System.Runtime.InteropServices;

namespace KeyWarden;
internal unsafe static partial class ProcessExtensions
{
	[LibraryImport("kernel32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static partial bool GetNamedPipeClientProcessId(nint pipe, out int clientProcessId);

	public static Process? GetProcess(this NamedPipeServerStream pipeServer)
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			IntPtr pipeHandle = pipeServer.SafePipeHandle.DangerousGetHandle();
			if (!GetNamedPipeClientProcessId(pipeHandle, out int processId))
				return null;

			try
			{
				return Process.GetProcessById(processId);
			}
			catch (ArgumentException) { }
		}

		return null;
	}
	public static IReadOnlyList<Process> GetParentProcesses(this NamedPipeServerStream pipeServer)
	{
		var set = new HashSet<Process>();
		var processes = new List<Process>();
		var p = pipeServer.GetProcess();

		while (p is not null)
		{
			if (!set.Add(p))
				break;

			processes.Add(p);
			p = p.GetParent();
		}
		return processes;
	}

	[LibraryImport("ntdll.dll")]
	private static partial int NtQueryInformationProcess(IntPtr processHandle, int processInformationClass, ref ProcessBasicInformation processInformation, int processInformationLength, out int returnLength);

	[StructLayout(LayoutKind.Sequential)]
	private struct ProcessBasicInformation
	{
		public IntPtr Reserved1;
		public IntPtr PebBaseAddress;
		public IntPtr Reserved2_0;
		public IntPtr Reserved2_1;
		public IntPtr UniqueProcessId;
		public IntPtr InheritedFromUniqueProcessId;
	}

	public static Process? GetParent(this Process process)
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			var handle = process.Handle;
			var pbi = new ProcessBasicInformation();
			int returnLength;
			int status = NtQueryInformationProcess(handle, 0, ref pbi, Marshal.SizeOf(pbi), out returnLength);
			if (status != 0)
				throw new Win32Exception(status);

			var pid = pbi.InheritedFromUniqueProcessId.ToInt32();
			try
			{
				return Process.GetProcessById(pid);
			}
			catch (ArgumentException) { }
		}

		return null;
	}
}
