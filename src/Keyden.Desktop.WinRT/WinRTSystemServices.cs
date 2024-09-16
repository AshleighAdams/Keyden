using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using Microsoft.Win32;

using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;

namespace Keyden.Desktop.WinRT;

[SupportedOSPlatform("windows10.0.17134.0")]
internal sealed class WinRTSystemServices : ISystemServices
{
	public WinRTSystemServices()
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
			var startupTask = StartupTask.GetAsync("KeydenStartup").GetAwaiter().GetResult();
			return startupTask.State switch
			{
				StartupTaskState.Enabled => true,
				StartupTaskState.EnabledByPolicy => true,
				_ => false,
			};
		}
		set
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				var startupTask = StartupTask.GetAsync("KeydenStartup").GetAwaiter().GetResult();

				if (value)
					startupTask.RequestEnableAsync().GetAwaiter().GetResult();
				else
					startupTask.Disable();
			}
		}
	}

	public bool IsAutomaticStart => AppInstance.GetActivatedEventArgs().Kind == ActivationKind.StartupTask;

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
