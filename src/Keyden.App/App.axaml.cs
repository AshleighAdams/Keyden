using System;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using Keyden.Views;
using Avalonia.Controls;
using System.Text.Json;
using System.Text;
using System.IO.Pipes;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Reflection.Metadata;
using System.Threading;

namespace Keyden;

public partial class App : Application
{
	public ISystemServices SystemServices { get; }
	public App()
	{
		SystemServices = new GenericSystemServices();
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

	public static string AppPipePath
	{
		get
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				return "keyden";

			if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
			{
				var uid = Unix.Getuid().ToString(CultureInfo.InvariantCulture);
				var socketDirectory = $"/run/user/{uid}";

				// check for legacy linux systems
				if (!Directory.Exists(socketDirectory))
					socketDirectory = Directory.Exists("/run") ? "/run" : "/var/run";

				if (!Directory.Exists(socketDirectory))
					return "keyden";

				socketDirectory += "/keyden";
				var socketPath = $"{socketDirectory}/keyden.sock";

				if (!Directory.Exists(socketDirectory))
					Directory.CreateDirectory(socketDirectory);
				return socketPath;
			}

			if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{
				var uid = Unix.Getuid().ToString(CultureInfo.InvariantCulture);
				return $"/var/run/{uid}-keyden.sock";
			}

			return "keyden";
		}
	}

	private async void MainAppPipeThread(CancellationToken ct)
	{
		using var pipeServer = new NamedPipeServerStream(
				pipeName: AppPipePath,
				direction: PipeDirection.InOut,
				maxNumberOfServerInstances: NamedPipeServerStream.MaxAllowedServerInstances,
				transmissionMode: PipeTransmissionMode.Byte,
				options: PipeOptions.Asynchronous | PipeOptions.WriteThrough | PipeOptions.CurrentUserOnly,
				inBufferSize: 1,
		outBufferSize: 1);

		while (!ct.IsCancellationRequested)
		{
			try
			{
				await pipeServer.WaitForConnectionAsync(ct);
				var byteRead = pipeServer.ReadByte();
				if (byteRead == 0x0A)
				{
					ShowMainWindow();
					pipeServer.WriteByte(0xA0);
					pipeServer.Flush();
				}
			}
			catch (OperationCanceledException) { }
			catch (IOException) { }
			finally
			{
				if (pipeServer.IsConnected)
					pipeServer.Disconnect();
			}
		}
	}

	private CancellationTokenSource AppPipeCts = new();

	public override void OnFrameworkInitializationCompleted()
	{
		if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime)
		{
			try
			{
				using var client = new NamedPipeClientStream(pipeName: AppPipePath);
				if (client.IsConnected)
				{
					client.ReadTimeout = 1000;
					client.WriteTimeout = 1000;
					client.WriteByte(0x0A);
					client.Flush();
					if (client.ReadByte() == 0xA0)
						Environment.Exit(0);
				}
			}
			catch (IOException) { }
			catch (TimeoutException) { }
		}

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

			MainAppPipeThread(AppPipeCts.Token);
			desktop.Exit += DesktopAppExit;
		}
		else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
			singleViewPlatform.MainView = new MainView();

		base.OnFrameworkInitializationCompleted();
	}

	private void DesktopAppExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
	{
		AppPipeCts.Cancel();
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
		SettingsWindow.Activate();
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
		AboutWindow.Activate();
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
		MainWindow.Activate();
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
