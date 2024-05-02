using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KlabTestFramework.System.Abstractions;

public interface IComponent : IAsyncDisposable
{
    IEnumerable<IComponent> Children { get; }

    IComponentConfig GetConfig();

    Task ResetAsync();

    Task InitializerAsync(object config);
}

public interface IComponent<TConfig> : IComponent where TConfig : IComponentConfig
{
    TConfig Config { get; }

    /// <summary>
    /// Initializes the component with the given configuration.
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    Task InitializeAsync(TConfig config);

    /// <summary>
    /// Initializes the component with the given configuration.
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    Task IComponent.InitializerAsync(object config)
    {
        return InitializeAsync((TConfig)config);
    }

    /// <summary>
    /// Basic wrapper
    /// </summary>
    /// <returns></returns>
    IComponentConfig IComponent.GetConfig()
    {
        return Config;
    }
}
