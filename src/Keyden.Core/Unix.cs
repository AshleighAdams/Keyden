using System.Runtime.InteropServices;

namespace Keyden;

public static partial class Unix
{
	[LibraryImport("libc", EntryPoint = "getuid", SetLastError = true)]
	public static partial uint Getuid();
}

//internal static partial class Linux
//{
//	[LibraryImport("libc", EntryPoint = "get_proc_stats", SetLastError = true)]
//	public static partial uint GetProcStats(int pid, proc_t processInfo);
//}
