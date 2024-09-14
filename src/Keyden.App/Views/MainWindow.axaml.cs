using Avalonia;
using Avalonia.Controls;
using Avalonia.Diagnostics;
using Avalonia.Platform;

using System.Runtime.InteropServices;

namespace Keyden.Views;

public partial class MainWindow : Window
{
	public MainWindow()
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
		{
			ExtendClientAreaChromeHints = ExtendClientAreaChromeHints.SystemChrome | ExtendClientAreaChromeHints.OSXThickTitleBar;
			ExtendClientAreaTitleBarHeightHint = 50;
		}
		else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			ExtendClientAreaChromeHints = ExtendClientAreaChromeHints.NoChrome;
			ExtendClientAreaTitleBarHeightHint = -1;
		}

		InitializeComponent();

#if DEBUG
		this.AttachDevTools(new DevToolsOptions()
		{
		});
#endif
	}
}
