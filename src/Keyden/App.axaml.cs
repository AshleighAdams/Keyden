using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using Keyden.ViewModels;
using Keyden.Views;
using System;

using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Linq;
namespace Keyden;

public partial class App : Application
{
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

	private SshAgent? Agent { get; set; }
	private MainWindow? MainWindow { get; set; }
	public override void OnFrameworkInitializationCompleted()
	{
		var collection = new ServiceCollection();
		collection.AddKeydenServices();

		Services = collection.BuildServiceProvider();

		Agent = Services.GetService<SshAgent>();

		if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
		{
			desktop.MainWindow = MainWindow = new MainWindow();

			if (desktop.Args?.Contains("--daemon") ?? false)
				MainWindow.Hide();
			else
				MainWindow.Show();

			if (
				(desktop.Args?.Contains("--sync") ?? false) &&
				MainWindow.Content is MainView view &&
				view.DataContext is MainViewModel mainModel)
			{
				mainModel.SyncKeys();
			}
				
		}
		else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
			singleViewPlatform.MainView = new MainView();

		base.OnFrameworkInitializationCompleted();
	}

	private void MenuSettings_Click(object? sender, EventArgs e)
	{
		if (MainWindow?.Content is not MainView view)
			return;

	}

	private void MenuAbout_Click(object? sender, EventArgs e)
	{
		if (MainWindow?.Content is not MainView view)
			return;

		new AboutWindow().ShowDialog(MainWindow);
	}

	private void MenuShow_Click(object? sender, EventArgs e)
	{
		MainWindow?.Show();
	}

	private void MenuExit_Click(object? sender, EventArgs e)
	{
		MainWindow?.Close();
	}
}
