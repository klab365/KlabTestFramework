using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Klab.Toolkit.Results;

namespace KlabTestFramework.System.Abstractions;

/// <summary>
/// Interface for managing system components.
/// </summary>
public interface ISystemManager : IAsyncDisposable
{
    Task<Result> InitializeAsync(string path, CancellationToken cancellationToken = default);

    Task<Result<TComponent>> GetComponentByIdAsync<TComponent>(string id, CancellationToken cancellationToken = default) where TComponent : IComponent;

    Task<Result<IEnumerable<TComponent>>> GetAllComponentsOfTypeAsync<TComponent>(CancellationToken cancellationToken = default) where TComponent : IComponent;

    Task<Result<IEnumerable<IComponent>>> GetAllComponentsAsync(CancellationToken cancellationToken = default);
}
