using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Keyden.Views
{
	public enum ExceptionWindowResult
	{
		Continue,
		Abort,
	}

	public partial class ExceptionWindow : Window
	{
		public static Task<ExceptionWindowResult> Prompt(string message, CancellationToken ct = default)
		{
			var window = new ExceptionWindow(message, ct);
			window.Show();
			return window.Result;
		}

		public string Message { get; private set; }

		private readonly TaskCompletionSource<ExceptionWindowResult> Tcs = new();
		public Task<ExceptionWindowResult> Result => Tcs.Task;

		public ExceptionWindow()
		{
			Message = "Some exception message goes here and should eventually wrap around";

			InitializeComponent();
		}

		private readonly CancellationTokenRegistration? CancelRegistration = null;
		public ExceptionWindow(string message, CancellationToken ct)
		{
			Message = message;

			InitializeComponent();

			CancelRegistration = ct.Register(() =>
			{
				Tcs.TrySetCanceled();
				Dispatcher.UIThread.Post(() => Close());
			});

			DenyButton.Click += DenyButton_Click;
			AuthButton.Click += AuthButton_Click;
			Closed += ExceptionWindow_Closed;
		}

		private void ExceptionWindow_Closed(object? sender, EventArgs e)
		{
			CancelRegistration?.Unregister();
			Tcs.TrySetResult(ExceptionWindowResult.Abort);
		}

		private void DenyButton_Click(object? sender, RoutedEventArgs e)
		{
			Tcs.TrySetResult(ExceptionWindowResult.Abort);
			Close();
		}

		private void AuthButton_Click(object? sender, RoutedEventArgs e)
		{
			Tcs.TrySetResult(ExceptionWindowResult.Continue);
			Close();
		}
	}
}
