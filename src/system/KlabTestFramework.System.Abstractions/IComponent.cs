using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Klab.Toolkit.Results;

namespace KlabTestFramework.System.Abstractions;

public interface IComponent : IAsyncDisposable
{
    IEnumerable<IComponent> Children { get; }

    IComponentConfig GetConfig();

    Task<Result> ResetAsync(CancellationToken cancellationToken = default);

    Task<Result> InitializeAsync(CancellationToken cancellationToken = default);
}

public interface IComponent<out TConfig> : IComponent where TConfig : IComponentConfig
{
    /// <summary>
    /// Generic Configuration of the component.
    /// </summary>
    TConfig Config { get; }

    /// <summary>
    /// Basic Function to get the configuration of the component without to implement on each class.
    /// </summary>
    /// <returns></returns>
    IComponentConfig IComponent.GetConfig()
    {
        return Config;
    }
}
