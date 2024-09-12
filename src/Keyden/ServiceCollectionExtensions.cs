using Microsoft.Extensions.DependencyInjection;

using Keyden.ViewModels;
using Avalonia.Controls;

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
		// transient view models
		collection.AddTransient<MainViewModel>();
		if (Design.IsDesignMode)
		{
			collection.AddTransient<ActivityViewModel, DesignActivityViewModel>();
			collection.AddTransient<KeyOptionsViewModel, DesignKeyOptionsViewModel>();
		}
		else
		{
			collection.AddTransient<ActivityViewModel>();
			collection.AddTransient<KeyOptionsViewModel>();
		}

		// singletons, both design and runtime
		collection.AddSingleton<AgentK>();
		collection.AddSingletonAlias<ISshAgentHandler, AgentK>();

		if (Design.IsDesignMode)
		//if (true)
		{
			collection.AddSingleton<DesignTimeKeyStore>();
			collection.AddSingletonAlias<ISshKeyStore, DesignTimeKeyStore>();
			collection.AddSingletonAlias<ISshKeyOptionsStore, DesignTimeKeyStore>();
			return;
		}

		// runtime only singletons
		collection.AddSingleton<OnePassCliSshKeyStore>();
		collection.AddSingletonAlias<ISshKeyStore, OnePassCliSshKeyStore>();
		collection.AddSingletonAlias<ISshKeyOptionsStore, OnePassCliSshKeyStore>();
		collection.AddSingleton<SshAgent>();
		collection.AddSingleton<SshAgentOptions>();
	}
}
