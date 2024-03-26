using Microsoft.Extensions.DependencyInjection;

using KeyWarden.ViewModels;

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
		// services
		collection.AddSingleton<ISshKeyStore, OnePassCliSshKeyStore>();
		collection.AddSingleton<AgentK>();
		collection.AddSingletonAlias<ISshAgentHandler, AgentK>();
		collection.AddSingleton<SshAgent>();
		collection.AddSingleton<SshAgentOptions>();

		// transient view models
		collection.AddTransient<MainViewModel>();
		collection.AddTransient<ActivityViewModel>();
	}
}
