using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Keyden;

public interface ISystemServices
{
	bool AutomaticallyStartApp { get; set; }
	bool IsAutomaticStart { get; }

	TimeSpan UserIdleDuration { get; }
	public event EventHandler<EventArgs> MachineLocked;

	Task<AuthenticationResult> TryAuthenticateUser();
}

public record struct AuthenticationResult
{
	public required bool Success { get; set; }

	[MemberNotNullWhen(false, nameof(Success))]
	public string? FailMessage { get; set; }
}
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
}
