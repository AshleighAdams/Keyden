using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Keyden.Views
{
	public partial class UnlockPrompt : Window
	{
		public KeydenSettings Settings { get; }
		private readonly TaskCompletionSource<bool> Tcs = new();
		public Task<bool> Result => Tcs.Task;

		public UnlockPrompt()
		{
			Settings = new();

			InitializeComponent();

			if (ActualTransparencyLevel == WindowTransparencyLevel.Mica)
			{
				Acrylic.Material.TintOpacity = 0;
				Acrylic.Material.MaterialOpacity = 0;
			}

			DenyButton.Click += DenyButton_Click;
			UnlockButton.Click += UnlockButton_Click;
			Closed += UnlockPrompt_Closed;

			Pin.AttachedToVisualTree += Pin_AttachedToVisualTree;
		}

		private readonly CancellationTokenRegistration? CancelRegistration = null;
		public UnlockPrompt(CancellationToken ct)
		{
			Settings = App.GetService<KeydenSettings>();

			InitializeComponent();

			if (ActualTransparencyLevel == WindowTransparencyLevel.Mica)
			{
				Acrylic.Material.TintOpacity = 0;
				Acrylic.Material.MaterialOpacity = 0;
			}

			CancelRegistration = ct.Register(() =>
			{
				Tcs.TrySetCanceled();
				Dispatcher.UIThread.Post(() => Close());
			});


			DenyButton.Click += DenyButton_Click;
			UnlockButton.Click += UnlockButton_Click;
			Closed += UnlockPrompt_Closed;

			Pin.AttachedToVisualTree += Pin_AttachedToVisualTree;
		}

		private void Pin_AttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
		{
			Pin.Focus();
		}

		private void UnlockPrompt_Closed(object? sender, EventArgs e)
		{
			CancelRegistration?.Unregister();
			Tcs.TrySetResult(true);
		}

		private void DenyButton_Click(object? sender, RoutedEventArgs e)
		{
			Tcs.TrySetResult(false);
			Close();
		}

		private void UnlockButton_Click(object? sender, RoutedEventArgs e)
		{
			if (Settings.CheckPin(Pin.Text ?? string.Empty))
			{
				Tcs.TrySetResult(true);
			}
			else
			{
				Pin.Text = string.Empty;
				Pin.Focus();
			}
		}
	}
}
