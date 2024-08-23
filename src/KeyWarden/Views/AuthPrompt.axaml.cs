using Avalonia.Controls;
using Avalonia.Threading;

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace KeyWarden.Views
{
	public partial class AuthPrompt : Window
	{
		public SshKey Key { get; private set; }
		public ClientInfo ClientInfo { get; private set; }
		public string AppName { get; private set; }

		private readonly TaskCompletionSource<AuthResult> Tcs = new();
		public Task<AuthResult> Result => Tcs.Task;

		public AuthPrompt()
		{
			Key = new() { Id = string.Empty, Name = "Primary" };
			ClientInfo = new()
			{
				Processes = [],
				Username = "Someone",
			};
			AppName = "Something";

			InitializeComponent();
		}

		private readonly CancellationTokenRegistration? CancelRegistration = null;
		public AuthPrompt(SshKey key, ClientInfo clientInfo, AuthRequired authRequired, CancellationToken ct)
		{
			Key = key;
			ClientInfo = clientInfo;
			AppName = clientInfo.ApplicationName;

			InitializeComponent();
			CancelRegistration = ct.Register(() =>
			{
				Tcs.TrySetCanceled();
				Dispatcher.UIThread.Post(() => Close());
			});

			DenyButton.Click += DenyButton_Click;
			AuthButton.Click += AuthButton_Click;
			Closed += AuthPrompt_Closed;
		}

		private void AuthPrompt_Closed(object? sender, System.EventArgs e)
		{
			CancelRegistration?.Unregister();
			Tcs.TrySetResult(new() { Success = false, Rejected = false });
		}

		private void DenyButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
		{
			Tcs.TrySetResult(new() { Success = false, Rejected = true });
			Close();
		}

		private void AuthButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
		{
			Tcs.TrySetResult(new()
			{
				Success = true,
				FreshAuthorization = true,
			});
			Close();
		}
	}
}
