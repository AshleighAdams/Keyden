using System;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using Keyden.ViewModels;
using Keyden.Views;
using Avalonia.Controls;
using System.Text.Json;
using System.Text;

namespace Keyden;

public partial class App : Application
{
	public ISystemServices SystemServices { get; }
	public App()
	{
		SystemServices = new NullSystemServices();
	}
	public App(ISystemServices systemServices)
	{
		SystemServices = systemServices;
	}

	public override void Initialize()
	{
		AvaloniaXamlLoader.Load(this);
	}

	private ServiceProvider? Services { get; set; }
	public static T GetService<T>() where
		T : class
	{
		if (Current is not App app)
			throw new Exception($"{nameof(Current)} is not an {nameof(App)}");

		return app.Services?.GetRequiredService<T>()
			?? throw new Exception("Service provider not yet created");
	}
	public static T GetKeyedService<T>(object? key) where
		T : class
	{
		if (Current is not App app)
			throw new Exception($"{nameof(Current)} is not an {nameof(App)}");

		return app.Services?.GetRequiredKeyedService<T>(key)
			?? throw new Exception("Service provider not yet created");
	}

	private SshAgent? Agent { get; set; }
	private KeydenSettings? Settings { get; set; }

	public override void OnFrameworkInitializationCompleted()
	{
		var collection = new ServiceCollection();

		collection.AddSingleton(SystemServices);
		collection.AddKeydenServices();

		Services = collection.BuildServiceProvider();

		Agent = Services.GetService<SshAgent>();
		Settings = Services.GetService<KeydenSettings>();

		if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
		{
			desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;

			bool isAutomaticStart =
				SystemServices.IsAutomaticStart ||
				desktop.Args?.Contains("--hide") == true;

			if (!isAutomaticStart)
				ShowMainWindow();
		}
		else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
			singleViewPlatform.MainView = new MainView();

		base.OnFrameworkInitializationCompleted();
	}

	private static SettingsWindow? SettingsWindow { get; set; }
	public static void ShowSettingsWindow()
	{
		if (SettingsWindow is null)
		{
			SettingsWindow = new SettingsWindow();
			SettingsWindow.Closed += (s, e) => SettingsWindow = null;
		}
		SettingsWindow.Show();
	}

	private static AboutWindow? AboutWindow { get; set; }
	public static void ShowAboutWindow()
	{
		if (AboutWindow is null)
		{
			AboutWindow = new AboutWindow();
			AboutWindow.Closed += (s, e) => AboutWindow = null;
		}
		AboutWindow.Show();
	}
	private static MainWindow? MainWindow { get; set; }
	public static void ShowMainWindow()
	{
		if (MainWindow is null)
		{
			MainWindow = new MainWindow();
			MainWindow.Closed += (s, e) => MainWindow = null;
		}
		MainWindow.Show();
	}

	private void MenuSettings_Click(object? sender, EventArgs e) => ShowSettingsWindow();
	private void MenuAbout_Click(object? sender, EventArgs e) => ShowAboutWindow();
	private void MenuShow_Click(object? sender, EventArgs e) => ShowMainWindow();

	private void MenuExit_Click(object? sender, EventArgs e)
	{
		if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
			desktop.Shutdown();
	}

	public static KeydenSettings CreateSettings(IFileSystem fileSystem)
	{
		const string settingsLocation = "settings.json";
		KeydenSettings settings;

		if (fileSystem.TryReadBytes(settingsLocation, out var contents))
		{
			try
			{
				settings = JsonSerializer.Deserialize(
					contents.Span,
					SettingsGenerationContext.Default.KeydenSettings) ?? new();
			}
			catch
			{
				settings = new();
			}
		}
		else
			settings = new();

		settings.PropertyChanged += (s, e) =>
		{
			var jsonString = JsonSerializer.Serialize(
				settings,
				SettingsGenerationContext.Default.KeydenSettings);
			fileSystem.TryWriteBytes(settingsLocation, Encoding.UTF8.GetBytes(jsonString));
		};

		return settings;
	}
}
