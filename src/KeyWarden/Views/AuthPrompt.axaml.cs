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

		private readonly TaskCompletionSource<bool> Tcs = new();
		public Task<bool> Result => Tcs.Task;

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
		public AuthPrompt(SshKey key, ClientInfo clientInfo, CancellationToken ct)
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
			Tcs.TrySetResult(false);
		}

		private void DenyButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
		{
			Tcs.TrySetResult(false);
			Close();
		}

		private void AuthButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
		{
			Tcs.TrySetResult(true);
			Close();
		}
	}
}
