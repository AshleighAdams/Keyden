using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Pipes;
using System.Runtime.InteropServices;

namespace Keyden;

internal unsafe static partial class ProcessExtensions
{
	public static Process? GetProcess(this NamedPipeServerStream pipeServer)
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			IntPtr pipeHandle = pipeServer.SafePipeHandle.DangerousGetHandle();
			if (!Win32.GetNamedPipeClientProcessId(pipeHandle, out int processId))
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

	public static Process? GetParent(this Process process)
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			try
			{
				var handle = process.Handle;
				var pbi = new Win32.ProcessBasicInformation();
				int returnLength;
				int status = Win32.NtQueryInformationProcess(handle, 0, ref pbi, Marshal.SizeOf(pbi), out returnLength);
				if (status != 0)
					throw new Win32Exception(status);

				var pid = pbi.InheritedFromUniqueProcessId.ToInt32();
				return Process.GetProcessById(pid);
			}
			catch (ArgumentException) { }
			catch (Win32Exception) { }
		}

		if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
		{
			/*
				#include <sys/sysctl.h>

				#define OPProcessValueUnknown UINT_MAX

				int ProcessIDForParentOfProcessID(int pid)
				{
					struct kinfo_proc info;
					size_t length = sizeof(struct kinfo_proc);
					int mib[4] = { CTL_KERN, KERN_PROC, KERN_PROC_PID, pid };
					if (sysctl(mib, 4, &info, &length, NULL, 0) < 0)
						return OPProcessValueUnknown;
					if (length == 0)
						return OPProcessValueUnknown;
					return info.kp_eproc.e_ppid;
				}
			*/
		}

		return null;
	}
}
