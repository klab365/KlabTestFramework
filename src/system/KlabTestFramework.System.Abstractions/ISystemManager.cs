using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Klab.Toolkit.Results;

namespace KlabTestFramework.System.Abstractions;

public interface ISystemManager : IAsyncDisposable
{
    IEnumerable<IComponent> Components { get; }

    Task<Result> InitializeAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a component by its id.
    /// </summary>
    /// <typeparam name="TComponent"></typeparam>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Result<TComponent>> GetValidComponentAsync<TComponent>(string id, CancellationToken cancellationToken = default) where TComponent : IComponent;

    /// <summary>
    /// Gets all components of the specified type.
    /// </summary>
    /// <typeparam name="TComponent"></typeparam>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Result<IEnumerable<TComponent>>> GetAllComponentsAsync<TComponent>(CancellationToken cancellationToken = default) where TComponent : IComponent;

    /// <summary>
    /// Gets all components.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Result<IEnumerable<IComponent>>> GetAllComponentsAsync(CancellationToken cancellationToken = default);
}
