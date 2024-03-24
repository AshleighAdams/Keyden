using Avalonia.Controls;

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
			Key = new() { Name = "Primary" };
			ClientInfo = new()
			{
				Processes = [],
				Username = "Someone",
			};
			AppName = "Something";

			InitializeComponent();
		}

		private string GetApplicationName()
		{
			Process? firstProcess = null;
			Process? lastProcess = null;
			foreach (var p in ClientInfo.Processes)
			{
				if (p.MainWindowHandle == nint.Zero)
					continue;

				firstProcess ??= p;
				lastProcess = p;
			}

			return firstProcess?.ProcessName ?? "Unknown";
		}

		private readonly CancellationTokenRegistration? CancelRegistration = null;
		public AuthPrompt(SshKey key, ClientInfo clientInfo, CancellationToken ct)
		{
			Key = key;
			ClientInfo = clientInfo;
			AppName = GetApplicationName();

			InitializeComponent();

			ct.Register(() =>
			{
				Close();
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
