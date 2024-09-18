using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;

namespace Keyden;

internal unsafe static partial class ProcessExtensions
{
	public static IReadOnlyList<Process> GetParentProcesses(this NamedPipeServerStream pipeServer, ISystemServices systemServices)
	{
		var set = new HashSet<Process>();
		var processes = new List<Process>();
		var p = systemServices.GetPipeClientProcess(pipeServer);

		while (p is not null)
		{
			if (!set.Add(p))
				break;
			if (processes.Count > 24)
				break;

			processes.Add(p);
			p = systemServices.GetParentProcess(p);
		}
		return processes;
	}
}
