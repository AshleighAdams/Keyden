using Avalonia.Controls;

namespace KeyWarden.Views
{
	public partial class AboutWindow : Window
	{
		public string Version => Verlite.Version.Full;
		public AboutWindow()
		{
			InitializeComponent();
		}
	}
}
