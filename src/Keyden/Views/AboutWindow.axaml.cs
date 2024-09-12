using Avalonia.Controls;

namespace Keyden.Views
{
	public partial class AboutWindow : Window
	{
		public string Version => Verlite.Version.Full;
		public AboutWindow()
		{
			InitializeComponent();

			if (ActualTransparencyLevel == WindowTransparencyLevel.Mica)
			{
				Acrylic.Material.TintOpacity = 0;
				Acrylic.Material.MaterialOpacity = 0;
			}
		}
	}
}
