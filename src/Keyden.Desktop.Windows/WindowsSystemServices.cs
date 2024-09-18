using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

using Keyden.Desktop.Windows;
using Keyden.Views;

using Microsoft.Win32;

using Renci.SshNet;

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

	public async Task<AuthResult> TryAuthUser(AuthRequired authRequired, ClientInfo clientInfo, SshKey key, CancellationToken ct)
	{
		AuthResult result = new(){ Success = true };
		AuthPrompt? window = null;
		try
		{
			if (authRequired.HasFlag(AuthRequired.Authentication))
			{
				window = new AuthPrompt(key, clientInfo, authRequired, ct);

				// position the window over the application requesting auth if possible:
				var clientWindow = clientInfo.MainProcess?.MainWindowHandle ?? nint.Zero;
				if (clientWindow != nint.Zero)
				{
					bool clientWindowIsFocused = Win32.GetForegroundWindow() == clientInfo.MainProcess!.MainWindowHandle;
					if (clientWindowIsFocused && Win32.GetWindowRect(clientWindow, out var clientRect))
					{
						window.Measure(new(double.PositiveInfinity, double.PositiveInfinity));

						var width = clientRect.Right - clientRect.Left;
						var height = clientRect.Bottom - clientRect.Top;
						var newX = (clientRect.Left + width / 2) - (int)(window.DesiredSize.Width / 2);
						var newY = (clientRect.Top + height / 2) - (int)(window.DesiredSize.Height / 2);

						window.Position = new(newX, newY);
						window.WindowStartupLocation = Avalonia.Controls.WindowStartupLocation.Manual;
					}
				}

				window.Show();
				result = await window.Result;
				window.Topmost = false;
			}

			if (result.Success && authRequired.HasFlag(AuthRequired.Authentication))
			{
				var (authenticated, message) = await TryAuthenticateUser();
				if (authenticated)
					result.FreshAuthentication = true;
				else
				{
					result.Success = false;
					result.Message = message;
				}
			}

			return result;
		}
		finally
		{
			window?.Close();
		}
	}

	private async Task<(bool success, string? message)> TryAuthenticateUser()
	{
		if (!KeyCredentialManager.IsSupportedAsync().GetAwaiter().GetResult())
			return (false, "Not supported");

		var got = await KeyCredentialManager.RequestCreateAsync("login", KeyCredentialCreationOption.ReplaceExisting);
		return got.Status switch
		{
			KeyCredentialStatus.Success => (true, null),
			KeyCredentialStatus.UnknownError => (false, "Unknown failure"),
			KeyCredentialStatus.NotFound => (false, "Not found"),
			KeyCredentialStatus.UserCanceled => (false, "User canceled"),
			KeyCredentialStatus.UserPrefersPassword => (false, "User prefers password"),
			KeyCredentialStatus.CredentialAlreadyExists => (false, "Already exists"),
			KeyCredentialStatus.SecurityDeviceLocked => (false, "Security device locked"),
			_ => (false, "Unknown failure (2)"),
		};
	}

	public void NotifyPreauthorizedKey(ClientInfo clientInfo, SshKey key)
	{
		// TODO: popup a notification if the setting is enabled
	}
}
