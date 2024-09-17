using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading.Tasks;

using Keyden.Desktop.Windows;

using Microsoft.Win32;

using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Security.Credentials;

namespace Keyden.Desktop.WinRT;

[SupportedOSPlatform("windows10.0.17134.0")]
internal sealed class WindowsSystemServices : ISystemServices
{
	private readonly bool IsImmersiveProcess;

	public WindowsSystemServices()
	{
		IsImmersiveProcess = Win32.IsImmersiveProcess(Environment.ProcessId);
		SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
	}
	private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
	{
		if (e.Reason == SessionSwitchReason.SessionLock)
			MachineLocked?.Invoke(this, e);
	}

	public bool AutomaticallyStartApp
	{
		get
		{
			if (IsImmersiveProcess)
			{
				var startupTask = StartupTask.GetAsync("KeydenStartup").GetAwaiter().GetResult();
				return startupTask.State switch
				{
					StartupTaskState.Enabled => true,
					StartupTaskState.EnabledByPolicy => true,
					_ => false,
				};
			}
			else
			{
				using RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", false)!;

				var path = Environment.ProcessPath;
				if (string.IsNullOrEmpty(path))
					return false;

				return key.GetValue("Keyden") as string == $"\"{path}\" --hide";
			}
		}
		set
		{
			if (IsImmersiveProcess)
			{
				var startupTask = StartupTask.GetAsync("KeydenStartup").GetAwaiter().GetResult();

				if (value)
					startupTask.RequestEnableAsync().GetAwaiter().GetResult();
				else
					startupTask.Disable();
			}
			else
			{
				using RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true)!;

				var path = Environment.ProcessPath;
				if (string.IsNullOrEmpty(path))
					return;

				if (value)
					key.SetValue("Keyden", $"\"{path}\" --hide");
				else
					key.DeleteValue("Keyden");
			}
		}
	}

	public bool IsAutomaticStart => IsImmersiveProcess && AppInstance.GetActivatedEventArgs().Kind == ActivationKind.StartupTask;

	public event EventHandler<EventArgs>? MachineLocked;

	public TimeSpan UserIdleDuration
	{
		get
		{
			var lastInputInfo = new Win32.LastInputInfo();
			lastInputInfo.Size = Marshal.SizeOf(lastInputInfo);
			lastInputInfo.Time = 0;

			if (!Win32.GetLastInputInfo(ref lastInputInfo))
				return TimeSpan.Zero;

			var idleTime = Environment.TickCount - lastInputInfo.Time;
			return TimeSpan.FromMilliseconds(idleTime);
		}
	}

	public async Task<AuthenticationResult> TryAuthenticateUser()
	{
		if (!KeyCredentialManager.IsSupportedAsync().GetAwaiter().GetResult())
			return new() { Success = false, FailMessage = "Not supported" };

		var got = await KeyCredentialManager.RequestCreateAsync("login", KeyCredentialCreationOption.ReplaceExisting);
		return got.Status switch
		{
			KeyCredentialStatus.Success => new() { Success = true },
			KeyCredentialStatus.UnknownError => new() { Success = false, FailMessage = "Unknown failure" },
			KeyCredentialStatus.NotFound => new() { Success = false, FailMessage = "Not found" },
			KeyCredentialStatus.UserCanceled => new() { Success = false, FailMessage = "User canceled" },
			KeyCredentialStatus.UserPrefersPassword => new() { Success = false, FailMessage = "User prefers password" },
			KeyCredentialStatus.CredentialAlreadyExists => new() { Success = false, FailMessage = "Already exists" },
			KeyCredentialStatus.SecurityDeviceLocked => new() { Success = false, FailMessage = "Security device locked" },
			_ => new() { Success = false, FailMessage = "Unknown failure (2)" },
		};
	}

	public Process? GetPipeClientProcess(NamedPipeServerStream pipeServer)
	{
		nint pipeHandle = pipeServer.SafePipeHandle.DangerousGetHandle();
		if (!Win32.GetNamedPipeClientProcessId(pipeHandle, out int processId))
			return null;

		try
		{
			return Process.GetProcessById(processId);
		}
		catch (ArgumentException) { }

		return null;
	}

	public Process? GetParentProcess(Process process)
	{
		try
		{
			var handle = process.Handle;
			var pbi = new Win32.ProcessBasicInformation();
			int status = Win32.NtQueryInformationProcess(handle, 0, ref pbi, Marshal.SizeOf(pbi), out var returnLength);
			if (status != 0)
				throw new Win32Exception(status);

			var pid = pbi.InheritedFromUniqueProcessId.ToInt32();
			return Process.GetProcessById(pid);
		}
		catch (ArgumentException) { }
		catch (Win32Exception) { }

		return null;

		// OSX equiv for future reference
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
}
