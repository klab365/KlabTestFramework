using System;
using KlabTestFramework.System.Abstractions;
using KlabTestFramework.System.Lib.Specifications;
using Microsoft.Extensions.DependencyInjection;

namespace KlabTestFramework.System.Lib;


public static class SystemModule
{
    public static IServiceCollection UseSystemLib(this IServiceCollection services, Action<SystemModuleConfiguration>? configurationCallback = default)
    {
        SystemModuleConfiguration configuration = new();
        configurationCallback?.Invoke(configuration); // apply configuration if provided

        // internal services
        services.AddSingleton<ISystemManager, SystemManager>();
        services.Add(new ServiceDescriptor(typeof(IComponentRepository), configuration.ComponentRepositoryType, configuration.ComponentRepositoryLifetime));
        services.AddTransient<ComponentFactory>();
        services.AddTransient<IComponentFactory, ComponentFactory>(r => r.GetRequiredService<ComponentFactory>());

        // register components
        foreach (ComponentSpecification specification in configuration.ComponentSpecifications)
        {
            services.RegisterComponent(specification);
        }

        return services;
    }

    private static void RegisterComponent(this IServiceCollection services, ComponentSpecification specification)
    {
        switch (specification.Lifetime)
        {
            case ServiceLifetime.Singleton:
                services.AddSingleton(specification.ComponentType);
                break;
            case ServiceLifetime.Scoped:
                services.AddScoped(specification.ComponentType);
                break;
            case ServiceLifetime.Transient:
                services.AddTransient(specification.ComponentType);
                break;
        }

        services.AddTransient(specification.ConfigType);
        services.AddTransient(_ => specification);
    }
}
