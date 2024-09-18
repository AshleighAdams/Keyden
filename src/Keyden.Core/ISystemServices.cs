using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace Keyden;

public interface ISystemServices
{
	bool AutomaticallyStartApp { get; set; }
	bool IsAutomaticStart { get; }

	TimeSpan UserIdleDuration { get; }
	public event EventHandler<EventArgs> MachineLocked;

	Task<AuthResult> TryAuthUser(
			AuthRequired authRequired,
			ClientInfo clientInfo,
			SshKey key,
			CancellationToken ct);
	Task<bool> TryUnlockSettings(object? requester, CancellationToken ct);
	void NotifyPreauthorizedKey(ClientInfo clientInfo, SshKey key);
	string AuthenticationBranding { get; }

	Process? GetPipeClientProcess(NamedPipeServerStream pipeServer);
	Process? GetParentProcess(Process process);
}

public struct AuthResult
{
	public bool Success { get; set; }
	public bool Rejected { get; set; }
	public bool FreshAuthorization { get; set; }
	public bool FreshAuthentication { get; set; }
	public string? Message { get; set; }
}

[Flags]
public enum AuthRequired
{
	None,
	Authorization,
	Authentication,
}
