using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace Keyden;

public interface ISystemServices
{
	bool AutomaticallyStartApp { get; set; }
	bool IsAutomaticStart { get; }

	TimeSpan UserIdleDuration { get; }
	public event EventHandler<EventArgs> MachineLocked;

	Task<AuthenticationResult> TryAuthenticateUser();

	Process? GetPipeClientProcess(NamedPipeServerStream pipeServer);
	Process? GetParentProcess(Process process);
}

public record struct AuthenticationResult
{
	public required bool Success { get; set; }

	[MemberNotNullWhen(false, nameof(Success))]
	public string? FailMessage { get; set; }
}
