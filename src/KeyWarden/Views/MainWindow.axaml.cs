using Avalonia;
using Avalonia.Controls;
using Avalonia.Diagnostics;

namespace KeyWarden.Views;

public partial class MainWindow : Window
{
	public MainWindow()
	{
		InitializeComponent();

#if DEBUG
		this.AttachDevTools(new DevToolsOptions()
		{
		});
#endif
	}
}
