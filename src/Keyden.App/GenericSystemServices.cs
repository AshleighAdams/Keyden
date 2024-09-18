using System;
using System.Diagnostics;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

using Keyden.Views;

namespace Keyden;

public sealed class GenericSystemServices : ISystemServices
{
	public bool AutomaticallyStartApp
	{
		get => false;
		set { }
	}
	public bool IsAutomaticStart => false;
	public event EventHandler<EventArgs> MachineLocked { add { } remove { } }
	public TimeSpan UserIdleDuration => TimeSpan.Zero;

	public Process? GetPipeClientProcess(NamedPipeServerStream pipeServer) => null;
	public Process? GetParentProcess(Process process) => null;

	public async Task<AuthResult> TryAuthUser(AuthRequired authRequired, ClientInfo clientInfo, SshKey key, CancellationToken ct)
	{
		AuthResult result = new() { Success = true };
		AuthPrompt? window = null;
		try
		{
			var settings = App.GetService<KeydenSettings>();
			if (authRequired.HasFlag(AuthRequired.Authorization) || settings.AuthenticationMode != AuthenticationMode.System)
			{
				window = new AuthPrompt(key, clientInfo, authRequired, ct);

				window.Show();
				window.Activate();
				result = await window.Result;
				window.Topmost = false;
			}

			bool authenticationRequired =
				result.Success &&
				result.FreshAuthentication == false &&
				authRequired.HasFlag(AuthRequired.Authentication);
			if (authenticationRequired)
			{
				await Task.Delay(1000);
				result.FreshAuthentication = true;
				result.Success = true;
			}

			return result;
		}
		finally
		{
			window?.Close();
		}
	}

	public void NotifyPreauthorizedKey(ClientInfo clientInfo, SshKey key)
	{
	}
	public string AuthenticationBranding => "None";
}
