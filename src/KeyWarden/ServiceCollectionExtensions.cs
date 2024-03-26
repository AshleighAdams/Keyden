using KeyWarden.ViewModels;

using Microsoft.Extensions.DependencyInjection;

namespace KeyWarden;

public static class ServiceCollectionExtensions
{
	public static void AddKeyWardenServices(this IServiceCollection collection)
	{
		// services
		collection.AddSingleton<ISshKeyStore, OnePassCliSshKeyStore>();
		collection.AddSingleton<ISshAgentHandler, AgentK>();
		collection.AddSingleton<AgentK>();
		collection.AddSingleton<SshAgent>();
		collection.AddSingleton<SshAgentOptions>();

		// transient view models
		collection.AddTransient<MainViewModel>();
		collection.AddTransient<ActivityViewModel>();
	}
}
