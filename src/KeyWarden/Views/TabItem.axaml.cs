using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;

using System;

namespace KeyWarden.Views
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

			TheButton.AddHandler(PointerPressedEvent, TheButton_Click, Avalonia.Interactivity.RoutingStrategies.Tunnel);
		}

		private void TheButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
		{
			if (ExpandableContent is not null)
				IsExpanded = !IsExpanded;
			else
				SelectedIndex = Index;
		}
	}
}
