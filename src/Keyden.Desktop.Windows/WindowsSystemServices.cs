using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading.Tasks;

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


}
