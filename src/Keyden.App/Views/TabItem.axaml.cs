using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Styling;
using Avalonia.Threading;

using System;
using System.Threading;

namespace Keyden.Views
{
	public partial class TabItem : UserControl
	{
		public static readonly StyledProperty<object?> ExpandableContentProperty =
			AvaloniaProperty.Register<TabItem, object?>(nameof(ExpandableContent));
		public object? ExpandableContent
		{
			get => GetValue(ExpandableContentProperty);
			set => SetValue(ExpandableContentProperty, value);
		}

		public static readonly StyledProperty<string> TextProperty
			= AvaloniaProperty.Register<TabItem, string>(nameof(Text), defaultValue: "Unnamed");
		public string Text
		{
			get => GetValue(TextProperty);
			set => SetValue(TextProperty, value);
		}

		public static readonly StyledProperty<string> IconProperty
			= AvaloniaProperty.Register<TabItem, string>(nameof(Icon), defaultValue: "fa-sync");
		public string Icon
		{
			get => GetValue(IconProperty);
			set => SetValue(IconProperty, value);
		}

		public static readonly DirectProperty<TabItem, int> IndexProperty
			= AvaloniaProperty.RegisterDirect<TabItem, int>(
				nameof(Index),
				static o => o.Index,
				static (o, v) => o.Index = v,
				unsetValue: 0);

		private int _Index;
		public int Index
		{
			get => _Index;
			set => SetAndRaise(IndexProperty, ref _Index, value);
		}

		public static readonly DirectProperty<TabItem, int> SelectedIndexProperty =
			AvaloniaProperty.RegisterDirect<TabItem, int>(
				name: nameof(SelectedIndex),
				static o => o.SelectedIndex,
				static (o, v) => o.SelectedIndex = v,
				unsetValue: -1,
				defaultBindingMode: BindingMode.TwoWay);

		private int _SelectedIndex = -1;
		public int SelectedIndex
		{
			get => _SelectedIndex;
			set
			{
				SetAndRaise(SelectedIndexProperty, ref _SelectedIndex, value);
				PseudoClasses.Set(":selected", SelectedIndex == Index);
			}
		}

		public static readonly StyledProperty<bool> IsExpandedProperty
			= AvaloniaProperty.Register<TabItem, bool>(nameof(IsExpanded), defaultValue: false);
		public bool IsExpanded
		{
			get => GetValue(IsExpandedProperty);
			set => SetValue(IsExpandedProperty, value);
		}

		public TabItem()
		{
			InitializeComponent();

			TheButton.AddHandler(PointerPressedEvent, TheButton_Click, RoutingStrategies.Tunnel);
		}

		private CancellationTokenSource? AnimationCts;
		private void TheButton_Click(object? sender, RoutedEventArgs e)
		{
			if (ExpandableContent is not null)
			{
				IsExpanded = !IsExpanded;
				var oldMaxHeight = double.IsPositiveInfinity(Resizer.MaxHeight) ? PART_ContentPresenter.DesiredSize.Height : Resizer.MaxHeight;
				var newMaxHeight = IsExpanded ? PART_ContentPresenter.DesiredSize.Height : 0.0;
				var finalMaxHeight = IsExpanded ? double.PositiveInfinity : 0.0;
				var duration = 0.1;

				var anim = new Animation()
				{
					Duration = TimeSpan.FromSeconds(duration),
					IterationCount = new IterationCount(1, IterationType.Many),
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
			else
				SelectedIndex = Index;
		}
	}
}
