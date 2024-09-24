using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Klab.Toolkit.Results;

namespace KlabTestFramework.System.Abstractions;

public interface IComponent : IAsyncDisposable
{
    IEnumerable<IComponent> Children { get; }

    IComponentConfig GetConfig();

    void SetConfig(object config);

    Task<Result> ResetAsync();

    Task<Result> InitializeAsync();
}

public interface IComponent<TConfig> : IComponent where TConfig : IComponentConfig
{
    /// <summary>
    /// Generic Configuration of the component.
    /// </summary>
    TConfig Config { get; set; }

    /// <summary>
    /// Basic Function to get the configuration of the component without to implement on each class.
    /// </summary>
    /// <returns></returns>
    IComponentConfig IComponent.GetConfig()
    {
        return Config;
    }

    void IComponent.SetConfig(object config)
    {
        if (config is not TConfig typedConfig)
        {
            throw new ArgumentException($"Invalid config type. Expected {typeof(TConfig).Name} but received {config.GetType().Name}");
        }

        Config = typedConfig;
    }
}
