using Microsoft.Extensions.DependencyInjection;

namespace KlabTestFramework.System.Lib;


public class SystemModule
{
    public static IServiceCollection UseSystemLib(this IServiceCollection services, Action<SystemModuleConfiguration>? configurationCallback = default)
    {
        SystemModuleConfiguration configuration = new();
        configurationCallback?.Invoke(configuration); // apply configuration if provided ;)

        services.AddSystemSubmodule(configuration);


        return services;
    }
}
