using Avalonia.Controls;

using System.Threading.Tasks;

namespace KeyWarden.Views
{
	public partial class AuthPrompt : Window
	{
		public SshKey Key { get; private set; }

		private readonly TaskCompletionSource<bool> Tcs = new();
		public Task<bool> Result => Tcs.Task;

		public AuthPrompt()
		{
			Key = new() { Name = "Primary" };
			InitializeComponent();
		}

		public AuthPrompt(SshKey key)
		{
			Key = key;

			InitializeComponent();

			DenyButton.Click += DenyButton_Click;
			AuthButton.Click += AuthButton_Click;
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
