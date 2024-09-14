using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;

using CommunityToolkit.Mvvm.ComponentModel;

using Projektanker.Icons.Avalonia;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Keyden.Views
{
	public partial class AuthPrompt : Window
	{
		public SshKey Key { get; private set; }
		public ClientInfo ClientInfo { get; private set; }
		public string AppName { get; private set; }

		public static readonly StyledProperty<bool> AuthButtonEnabledProperty = AvaloniaProperty.Register<Window, bool>(nameof(AuthButtonEnabled), false);
		public bool AuthButtonEnabled
		{
			get => GetValue(AuthButtonEnabledProperty);
			set => SetValue(AuthButtonEnabledProperty, value);
		}

		public static readonly StyledProperty<string> AuthButtonIconProperty = AvaloniaProperty.Register<Window, string>(nameof(AuthButtonIcon), "fa-fingerprint");
		public string AuthButtonIcon
		{
			get => GetValue(AuthButtonIconProperty);
			set => SetValue(AuthButtonIconProperty, value);
		}

		public static readonly StyledProperty<bool> AuthButtonIconVisibleProperty = AvaloniaProperty.Register<Window, bool>(nameof(AuthButtonIconVisible), false);
		public bool AuthButtonIconVisible
		{
			get => GetValue(AuthButtonIconVisibleProperty);
			set => SetValue(AuthButtonIconVisibleProperty, value);
		}

		public static readonly StyledProperty<IconAnimation> AuthButtonIconAnimationProperty = AvaloniaProperty.Register<Window, IconAnimation>(nameof(AuthButtonIconAnimation), IconAnimation.None);
		public IconAnimation AuthButtonIconAnimation
		{
			get => GetValue(AuthButtonIconAnimationProperty);
			set => SetValue(AuthButtonIconAnimationProperty, value);
		}

		public string AuthButtonText { get; private set; } = "Authorize";

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

			if (ActualTransparencyLevel == WindowTransparencyLevel.Mica)
			{
				Acrylic.Material.TintOpacity = 0;
				Acrylic.Material.MaterialOpacity = 0;
			}
		}

		private readonly CancellationTokenRegistration? CancelRegistration = null;
		public AuthPrompt(
			KeydenSettings settings,
			SshKey key,
			ClientInfo clientInfo,
			AuthRequired authRequired,
			CancellationToken ct)
		{
			Key = key;
			ClientInfo = clientInfo;
			AppName = clientInfo.ApplicationName;
			AuthButtonIconVisible = true;

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

			if (settings.AuthButtonEnableDelay == 0)
				AuthButtonEnabled = true;
			else
				_ = Task.Run(async () =>
				{
					var ms = (int)(settings.AuthButtonEnableDelay * 1000);
					await Task.Delay(ms, ct);
					Dispatcher.UIThread.Post(() => AuthButtonEnabled = true);
				}, ct);

			if (authRequired.HasFlag(AuthRequired.Authorization))
			{
				AuthButtonIcon = "fa-fingerprint";
				AuthButtonIconVisible = true;
			}

			DenyButton.Click += DenyButton_Click;
			AuthButton.Click += AuthButton_Click;
			Closed += AuthPrompt_Closed;
		}

		private void AuthPrompt_Closed(object? sender, EventArgs e)
		{
			AuthButtonEnabled = false;
			CancelRegistration?.Unregister();
			Tcs.TrySetResult(new() { Success = false, Rejected = false });
		}

		private void DenyButton_Click(object? sender, RoutedEventArgs e)
		{
			AuthButtonEnabled = false;
			Tcs.TrySetResult(new() { Success = false, Rejected = true });
			Close();
		}

		private void AuthButton_Click(object? sender, RoutedEventArgs e)
		{
			AuthButtonEnabled = false;
			AuthButtonIcon = "mdi-loading";
			AuthButtonIconVisible = true;
			AuthButtonIconAnimation = IconAnimation.Spin;

			Tcs.TrySetResult(new()
			{
				Success = true,
				FreshAuthorization = true,
			});
		}
	}
}
