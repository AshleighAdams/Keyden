using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Platform;
using Avalonia.Styling;

using Keyden.ViewModels;

using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Keyden.Views;

public partial class SettingsWindow : Window
{
	public SettingsWindow()
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

		DataContext = App.GetService<SettingsViewModel>();

		InitializeComponent();

		RefreshAcrylicProperties();

		ActualThemeVariantChanged += Window_ActualThemeVariantChanged;
	}

	private void Window_ActualThemeVariantChanged(object? sender, EventArgs e)
	{
		RefreshAcrylicProperties();
	}

	private void RefreshAcrylicProperties()
	{
		if (VisualRoot is not Window window)
			return;

		if (window.ActualTransparencyLevel == WindowTransparencyLevel.Mica)
		{
			if (window.ActualThemeVariant == ThemeVariant.Light)
			{
				Acrylic1.Material.TintOpacity = 1;
				Acrylic1.Material.MaterialOpacity = 1;
			}
			else
			{
				Acrylic1.Material.TintOpacity = 1;
				Acrylic1.Material.MaterialOpacity = 0;
			}

			Acrylic2.Material.TintOpacity = 0;
			Acrylic2.Material.MaterialOpacity = 0;
		}
	}
}

public class EnumDescriptionConverter : IValueConverter
{
	object IValueConverter.Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		return value switch
		{
			KeystoreBackend backend => backend switch
			{
				KeystoreBackend.None => "None",
				KeystoreBackend.DeveloperTest => "Developer Test",
				KeystoreBackend.OnePassCLI => "1Password CLI",
				_ => backend.ToString(),
			},
			_ => value?.ToString() ?? string.Empty,
		};
	}

	object IValueConverter.ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		return string.Empty;
	}
}
