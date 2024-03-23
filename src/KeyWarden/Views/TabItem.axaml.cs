using Avalonia;
using Avalonia.Controls;

namespace KeyWarden.Views
{
	public partial class TabItem : UserControl
	{
		public static readonly StyledProperty<string> TextProperty
			= AvaloniaProperty.Register<TabItem, string>(nameof(Text), "Tab name");

		public static readonly StyledProperty<string> IconProperty
			= AvaloniaProperty.Register<TabItem, string>(nameof(Icon), "fa-sync");

		public string Text
		{
			get => GetValue(TextProperty);
			set => SetValue(TextProperty, value);
		}
		public string Icon
		{
			get => GetValue(IconProperty);
			set => SetValue(IconProperty, value);
		}

		public TabItem()
		{
			InitializeComponent();
		}
	}
}
