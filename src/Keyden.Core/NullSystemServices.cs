using System;
using System.Diagnostics;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace Keyden;

public sealed class NullSystemServices : ISystemServices
{
	public bool AutomaticallyStartApp
	{
		get => false;
		set { }
	}
	public bool IsAutomaticStart => false;
	public event EventHandler<EventArgs> MachineLocked { add { } remove { } }
	public TimeSpan UserIdleDuration => TimeSpan.Zero;

	public Task<AuthenticationResult> TryAuthenticateUser()
	{
		return Task.FromResult<AuthenticationResult>(new() { Success = false, FailMessage = "Not supported" });
	}

	public Process? GetPipeClientProcess(NamedPipeServerStream pipeServer) => null;
	public Process? GetParentProcess(Process process) => null;
}
