using Microsoft.Extensions.DependencyInjection;

using Keyden.ViewModels;
using Avalonia.Controls;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Keyden;

public static class ServiceCollectionExtensions
{
	public static void AddSingletonAlias<TBase, TDerived>(this IServiceCollection collection)
		where TBase : class
		where TDerived : TBase
	{
		collection.AddSingleton<TBase>(x => x.GetRequiredService<TDerived>());
	}

	public static void AddKeydenServices(this IServiceCollection collection)
	{
		bool isDesigner = Design.IsDesignMode;

		collection.AddSingleton((provider) => App.CreateSettings(provider.GetRequiredService<IFileSystem>()));

		//  view models, singletons = preserve their view state
		collection.AddSingleton<MainViewModel>(); // singleton to preserve state
		if (isDesigner)
		{
			collection.AddTransient<ActivityViewModel, DesignActivityViewModel>();
			collection.AddTransient<KeyOptionsViewModel, DesignKeyOptionsViewModel>();
			collection.AddTransient<SettingsViewModel, DesignSettingsViewModel>();
		}
		else
		{
			collection.AddSingleton<ActivityViewModel>();
			collection.AddTransient<KeyOptionsViewModel>();
			collection.AddSingleton<SettingsViewModel>();
		}

		// singletons, both design and runtime
		collection.AddSingleton<AgentK>();
		collection.AddSingletonAlias<ISshAgentHandler, AgentK>();

		if (isDesigner)
		{
			collection.AddSingleton<IFileSystem, NullFileSystem>();
			collection.AddSingleton<IUserActivityTracker, NullUserActivityTracker>();
			collection.AddSingleton<DesignTimeKeyStore>();
			collection.AddSingletonAlias<ISshKeyStore, DesignTimeKeyStore>();
			collection.AddSingletonAlias<ISshKeyOptionsStore, DesignTimeKeyStore>();
		}
		else
		{
			collection.AddSingleton<IFileSystem>((provider) =>
			{
				var basePath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Keyden");

				try
				{
					if (!Directory.Exists(basePath))
						Directory.CreateDirectory(basePath);
				}
				catch (Exception) { }

				return new SystemFileSystem(basePath);
			});

			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				collection.AddSingleton<IUserActivityTracker, Win32UserActivityTracker>();
			else
				collection.AddSingleton<IUserActivityTracker, NullUserActivityTracker>();

			collection.AddKeyedSingleton<DeveloperTestKeyStore>("devtest");
			collection.AddKeyedSingleton<OnePassCliSshKeyStore>("op");

			collection.AddSingleton<KeyStoreController>();
			collection.AddSingletonAlias<ISshKeyStore, KeyStoreController>();
			collection.AddSingletonAlias<ISshKeyOptionsStore, KeyStoreController>();

			collection.AddSingleton<SshAgent>();
			collection.AddSingleton<SshAgentOptions>();
		}
	}
}
