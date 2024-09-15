using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using Microsoft.Win32;

namespace Keyden.Desktop.Any;

[SupportedOSPlatform("windows")]
internal sealed class Win32SystemServices : ISystemServices
{
	public Win32SystemServices()
	{
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
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				using RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", false)!;

				var path = Environment.ProcessPath;
				if (string.IsNullOrEmpty(path))
					return false;

				return key.GetValue("Keyden") as string == $"\"{path}\" --hide";
			}
			return false;
		}
		set
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				using RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true)!;

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
}
