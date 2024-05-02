using System;
using System.Collections.Generic;
using System.Linq;
using KlabTestFramework.System.Abstractions;
using KlabTestFramework.System.Lib.Infrastructure;
using KlabTestFramework.System.Lib.Specifications;
using KlabTestFramework.System.Lib.System;
using Microsoft.Extensions.DependencyInjection;

namespace KlabTestFramework.System.Lib;


public static class SystemModule
{
    public static IServiceCollection UseSystemLib(this IServiceCollection services, Action<SystemModuleConfiguration>? configurationCallback = default)
    {
        SystemModuleConfiguration configuration = new();
        configurationCallback?.Invoke(configuration); // apply configuration if provided ;)

        RegisterSubmodules(services, configuration);

        // internal services
        services.AddSingleton<ISystemManager, SystemManager>();
        services.AddTransient<IComponentRepository, ComponentTomlRepository>();
        services.AddTransient<ComponentFactory>();
        return services;
    }

    private static void RegisterSubmodules(IServiceCollection services, SystemModuleConfiguration configuration)
    {
        IEnumerable<ComponentSpecification> specifications = configuration.Submodules.SelectMany(s => s.GetSpecifications());
        foreach (ComponentSpecification? specification in specifications)
        {
            services.AddTransient(_ => specification);
        }
    }
}
