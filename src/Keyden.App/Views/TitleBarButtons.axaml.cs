using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;

using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Keyden.Views
{
	public partial class TitleBarButtons : UserControl, IObserver<WindowState>
	{
		public TitleBarButtons()
		{
			InitializeComponent();

			if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{
				MinimizeButton.IsVisible = false;
				MaximizeButton.IsVisible = false;
				CloseButton.IsVisible = false;
				return;
			}

			MinimizeButton.Click += MinimizeWindow;
			MaximizeButton.Click += MaximizeWindow;
			CloseButton.Click += CloseWindow;

			SubscribeToWindowState();
		}


		protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
		{
			base.OnAttachedToVisualTree(e);

			if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
				return;

			if (e.Root is not Window window)
				return;

			bool usingClientChrome = window.IsExtendedIntoWindowDecorations;
			MinimizeButton.IsVisible = usingClientChrome;
			MaximizeButton.IsVisible = usingClientChrome;
			CloseButton.IsVisible = usingClientChrome;

			window.PropertyChanged += Window_PropertyChanged;
		}

		protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
		{
			base.OnDetachedFromVisualTree(e);

			if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
				return;

			if (e.Root is not Window window)
				return;

			window.PropertyChanged -= Window_PropertyChanged;
		}

		private void Window_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
		{
			if (e.Property == Window.IsExtendedIntoWindowDecorationsProperty)
			{
				if (e.NewValue is not bool newValue)
					return;

				if (newValue)
				{
					MinimizeButton.IsVisible = true;
					MaximizeButton.IsVisible = true;
					CloseButton.IsVisible = true;
				}
				else
				{
					MinimizeButton.IsVisible = false;
					MaximizeButton.IsVisible = false;
					CloseButton.IsVisible = false;
				}
			}
		}

		private void CloseWindow(object? sender, RoutedEventArgs e)
		{
			HostWindow ??= (Window?)VisualRoot;
			if (HostWindow is null)
				return;

			HostWindow.Close();
		}

		private void MaximizeWindow(object? sender, RoutedEventArgs e)
		{
			HostWindow ??= (Window?)VisualRoot;
			if (HostWindow is null)
				return;

			if (HostWindow.WindowState == WindowState.Normal)
				HostWindow.WindowState = WindowState.Maximized;
			else
				HostWindow.WindowState = WindowState.Normal;
		}

		private void MinimizeWindow(object? sender, RoutedEventArgs e)
		{
			HostWindow ??= (Window?)VisualRoot;
			if (HostWindow is null)
				return;

			HostWindow.WindowState = WindowState.Minimized;
		}

		void IObserver<WindowState>.OnCompleted() { }
		void IObserver<WindowState>.OnError(Exception error) { }
		void IObserver<WindowState>.OnNext(WindowState state)
		{
			ArgumentNullException.ThrowIfNull(HostWindow);

			switch (state)
			{
				case WindowState.FullScreen:
				case WindowState.Maximized:
					MaximizeIcon.Data = Geometry.Parse("M2048 1638h-410v410h-1638v-1638h410v-410h1638v1638zm-614-1024h-1229v1229h1229v-1229zm409-409h-1229v205h1024v1024h205v-1229z");
					HostWindow.Padding = new Thickness(7, 7, 7, 7);
					MaximizeToolTip.Content = "Restore Down";
					break;
				case WindowState.Normal:
					MaximizeIcon.Data = Geometry.Parse("M2048 2048v-2048h-2048v2048h2048zM1843 1843h-1638v-1638h1638v1638z");
					HostWindow.Padding = new Thickness(0, 0, 0, 0);
					MaximizeToolTip.Content = "Maximize";
					break;
				default:
					break;
			}
		}

		private Window? HostWindow { get; set; }
		private IDisposable? ObservableSubscription { get; set; }
		private async void SubscribeToWindowState()
		{
			while (HostWindow is null)
			{
				HostWindow = (Window?)VisualRoot;
				await Task.Delay(50);
			}

			ObservableSubscription = HostWindow.GetObservable(Window.WindowStateProperty).Subscribe(this);
		}

		protected override void OnUnloaded(Avalonia.Interactivity.RoutedEventArgs e)
		{
			ObservableSubscription?.Dispose();
			ObservableSubscription = null;
			base.OnUnloaded(e);
		}
	}
}
