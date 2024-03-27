using Microsoft.Extensions.DependencyInjection;

using KeyWarden.ViewModels;
using Avalonia.Controls;

namespace KeyWarden;

public static class ServiceCollectionExtensions
{
	public static void AddSingletonAlias<TBase, TDerived>(this IServiceCollection collection)
		where TBase : class
		where TDerived : TBase
	{
		collection.AddSingleton<TBase>(x => x.GetRequiredService<TDerived>());
	}

	public static void AddKeyWardenServices(this IServiceCollection collection)
	{
		// transient view models
		collection.AddTransient<MainViewModel>();
		collection.AddTransient<ActivityViewModel>();

		// singletons, both design and runtime
		collection.AddSingleton<AgentK>();
		collection.AddSingletonAlias<ISshAgentHandler, AgentK>();

		if (Design.IsDesignMode)
		{
			collection.AddSingleton<ISshKeyStore, DesignTimeKeyStore>();
			return;
		}

		// runtime only singletons
		collection.AddSingleton<ISshKeyStore, OnePassCliSshKeyStore>();
		collection.AddSingleton<SshAgent>();
		collection.AddSingleton<SshAgentOptions>();
	}
}
