using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Styling;
using Avalonia.Threading;

using Projektanker.Icons.Avalonia;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Keyden.Views
{
	public partial class AuthPrompt : Window
	{
		public SshKey Key { get; }
		public ClientInfo ClientInfo { get; }
		public KeydenSettings Settings { get; }
		public string AppName { get; }
		public string AppUser { get; }
		public string ProcessHierarchy { get; }


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

		public static readonly StyledProperty<string> AuthButtonTextProperty = AvaloniaProperty.Register<Window, string>(nameof(AuthButtonEnabled), "Authorize");
		public string AuthButtonText
		{
			get => GetValue(AuthButtonTextProperty);
			set => SetValue(AuthButtonTextProperty, value);
		}

		public static readonly StyledProperty<bool> IsExpandedProperty = AvaloniaProperty.Register<TabItem, bool>(nameof(IsExpanded), defaultValue: false);
		public bool IsExpanded
		{
			get => GetValue(IsExpandedProperty);
			set => SetValue(IsExpandedProperty, value);
		}

		private readonly AuthRequired AuthRequired;
		private readonly TaskCompletionSource<AuthResult> Tcs = new();
		private bool PinMode { get; } = false;
		public Task<AuthResult> Result => Tcs.Task;

		public AuthPrompt()
		{
			Key = new() { Id = string.Empty, Name = "Primary" };
			Settings = new();
			ClientInfo = new()
			{
				Processes = [],
				Username = "Someone",
			};
			AppName = "Something";
			AppUser = "Someone";
			ProcessHierarchy = "Something, bash, git-bash, Code, Explorer";
			AuthRequired = AuthRequired.Authorization | AuthRequired.Authorization;
			AuthButtonEnabled = true;

			InitializeComponent();

			KeyTitleBar.PointerPressed += KeyTitleBar_PointerPressed;
			KeyTitleBar.PointerEntered += KeyTitleBar_PointerEntered;
			KeyTitleBar.PointerExited += KeyTitleBar_PointerExited;

			if (ActualTransparencyLevel == WindowTransparencyLevel.Mica)
			{
				Acrylic.Material.TintOpacity = 0;
				Acrylic.Material.MaterialOpacity = 0;
			}
		}

		private readonly CancellationTokenRegistration? CancelRegistration = null;
		public AuthPrompt(
			SshKey key,
			ClientInfo clientInfo,
			AuthRequired authRequired,
			CancellationToken ct)
		{
			Key = key;
			ClientInfo = clientInfo;
			Settings = App.GetService<KeydenSettings>();
			AppName = clientInfo.ApplicationName;
			AppUser = clientInfo.Username;
			ProcessHierarchy = clientInfo.Processes.Count > 0
				? string.Join(", ", clientInfo.Processes.Select(static p => p.ProcessName).CompactDuplicates())
				: "Unknown";
			AuthRequired = authRequired;

			if (AuthRequired.HasFlag(AuthRequired.Authentication) && Settings.AuthenticationMode == AuthenticationMode.InternalPIN)
			{
				PinMode = true;
				AuthButtonIconVisible = false;
				AuthButtonText = "Authenticate";
			}
			else if (AuthRequired.HasFlag(AuthRequired.Authentication))
			{
				AuthButtonIcon = "fa-fingerprint";
				AuthButtonIconVisible = true;
				AuthButtonText = "Authenticate";
			}
			else
				AuthButtonIconVisible = false;

			InitializeComponent();

			KeyTitleBar.PointerPressed += KeyTitleBar_PointerPressed;
			KeyTitleBar.PointerEntered += KeyTitleBar_PointerEntered;
			KeyTitleBar.PointerExited += KeyTitleBar_PointerExited;

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

			var settings = App.GetService<KeydenSettings>();
			if (settings.AuthButtonEnableDelay == 0)
				AuthButtonEnabled = true;
			else
				_ = Task.Run(async () =>
				{
					var ms = (int)(settings.AuthButtonEnableDelay * 1000);
					await Task.Delay(ms, ct);
					Dispatcher.UIThread.Post(() => AuthButtonEnabled = true);
				}, ct);


			DenyButton.Click += DenyButton_Click;
			AuthButton.Click += AuthButton_Click;
			Closed += AuthPrompt_Closed;

			Pin.IsVisible = PinMode;
			if (PinMode)
				Pin.AttachedToVisualTree += Pin_AttachedToVisualTree;
		}

		private void Pin_AttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
		{
			if (PinMode)
				Pin.Focus();
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
			if (AuthRequired.HasFlag(AuthRequired.Authentication) && Settings.AuthenticationMode == AuthenticationMode.InternalPIN)
			{
				if (Settings.CheckPin(Pin.Text ?? string.Empty))
				{
					AuthButtonEnabled = false;
					AuthButtonIcon = "mdi-loading";
					AuthButtonIconVisible = true;
					AuthButtonIconAnimation = IconAnimation.Spin;
					Pin.IsEnabled = false;

					Tcs.TrySetResult(new()
					{
						Success = true,
						FreshAuthorization = AuthRequired.HasFlag(AuthRequired.Authorization),
						FreshAuthentication = AuthRequired.HasFlag(AuthRequired.Authentication),
					});
				}
				else
				{
					Pin.Text = string.Empty;
					Pin.Focus();
				}
			}
			else
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

		private CancellationTokenSource? AnimationCts;
		private readonly CubicEaseInOut AnimationEasing = new();
		private void KeyTitleBar_PointerPressed(object? sender, PointerPressedEventArgs e)
		{
			IsExpanded = !IsExpanded;

			if (IsExpanded)
				Chevron.Classes.Add("rotate");
			else
				Chevron.Classes.Remove("rotate");

			var oldMaxHeight = double.IsPositiveInfinity(Resizer.MaxHeight) ? CollapsableContent.DesiredSize.Height : Resizer.MaxHeight;
			var newMaxHeight = IsExpanded ? CollapsableContent.DesiredSize.Height : 0.0;
			var finalMaxHeight = IsExpanded ? double.PositiveInfinity : 0.0;
			var duration = 0.1;

			var anim = new Animation()
			{
				Duration = TimeSpan.FromSeconds(duration),
				IterationCount = new IterationCount(1, IterationType.Many),
				Easing = AnimationEasing,
				Children =
					{
						new KeyFrame()
						{
							Setters = { new Setter{ Property = MaxHeightProperty, Value = oldMaxHeight } },
							KeyTime = TimeSpan.FromSeconds(0.0),
						},
						new KeyFrame()
						{
							Setters = { new Setter{ Property = MaxHeightProperty, Value = newMaxHeight } },
							KeyTime = TimeSpan.FromSeconds(duration),
						},
					},
			};

			AnimationCts?.Cancel();
			// before starting the animation, the new final value must be set
			Resizer.MaxHeight = finalMaxHeight;
			AnimationCts = new CancellationTokenSource();
			_ = anim.RunAsync(Resizer, AnimationCts.Token);
		}

		private void KeyTitleBar_PointerEntered(object? sender, PointerEventArgs e)
		{
			KeyTitleBar.Parent?.Parent?.Classes.Add("hover");
		}

		private void KeyTitleBar_PointerExited(object? sender, PointerEventArgs e)
		{
			KeyTitleBar.Parent?.Parent?.Classes.Remove("hover");
		}
	}
}
