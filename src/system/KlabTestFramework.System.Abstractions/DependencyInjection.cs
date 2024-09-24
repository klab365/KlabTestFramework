using Microsoft.Extensions.DependencyInjection;

namespace KlabTestFramework.System.Abstractions;

public static class DependencyInjection
{
    public static void RegisterComponent(this IServiceCollection services, ComponentSpecification specification)
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
