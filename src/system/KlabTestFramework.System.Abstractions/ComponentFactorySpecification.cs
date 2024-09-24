using System;
using Microsoft.Extensions.DependencyInjection;

namespace KlabTestFramework.System.Abstractions;

public record ComponentSpecification
{
    public string TypeKey { get; }

    public Type ConfigType { get; }

    public Type ComponentType { get; }

    public ServiceLifetime Lifetime { get; }

    private ComponentSpecification(string key, Type configType, Type componentType, ServiceLifetime lifetime)
    {
        TypeKey = key;
        ConfigType = configType;
        ComponentType = componentType;
        Lifetime = lifetime;
    }

    public static ComponentSpecification Create<TConfig, TComponent>(string? key = default, ServiceLifetime lifetime = ServiceLifetime.Transient)
        where TConfig : IComponentConfig
        where TComponent : IComponent<TConfig>
    {
        if (key is null)
        {
            key = typeof(TComponent).Name;
        }

        return new ComponentSpecification(key, typeof(TConfig), typeof(TComponent), lifetime);
    }
}
