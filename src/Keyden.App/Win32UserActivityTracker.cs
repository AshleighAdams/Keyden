using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using Microsoft.Win32;

namespace Keyden;

[SupportedOSPlatform("windows")]
public sealed class Win32UserActivityTracker : IUserActivityTracker
{
	public Win32UserActivityTracker()
	{
		SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
	}

	private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
	{
		if (e.Reason == SessionSwitchReason.SessionLock)
			MachineLocked?.Invoke(this, e);
	}

	public event EventHandler<EventArgs>? MachineLocked;

	public TimeSpan IdleDuration
	{
		get
		{
			var lastInputInfo = new Win32.LastInputInfo();
			lastInputInfo.Size = Marshal.SizeOf(lastInputInfo);
			lastInputInfo.Time = 0;

			if (!Win32.GetLastInputInfo(ref lastInputInfo))
				return  TimeSpan.Zero;

			var idleTime = Environment.TickCount - lastInputInfo.Time;
			return TimeSpan.FromMilliseconds(idleTime);
		}
	}
}


[SupportedOSPlatform("windows")]
public static partial class Win32
{
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
}
