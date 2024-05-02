using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Klab.Toolkit.Results;
using KlabTestFramework.System.Abstractions;
using KlabTestFramework.System.Lib.Specifications;

namespace KlabTestFramework.System.Lib.System;

internal sealed class SystemManager : ISystemManager
{
    private readonly IComponentRepository _repository;
    private readonly ComponentFactory _componentFactory;
    private readonly List<IComponent> _components = new();

    public IEnumerable<IComponent> Components => _components;

    public SystemManager(
        IComponentRepository repository,
        ComponentFactory componentFactory)
    {
        _repository = repository;
        _componentFactory = componentFactory;
    }

    public async Task<Result> InitializeAsync(string path, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return SystemManagerErrors.Cancled;
        }

        if (string.IsNullOrWhiteSpace(path))
        {
            return SystemManagerErrors.PathIsRequired;
        }

        ComponentData[] componentData = await _repository.GetComponentAsync(path);
        _components.Clear();
        foreach (ComponentData data in componentData)
        {
            Result<IComponent> res = await _componentFactory.CreateComponentAsync(data);
            if (res.IsFailure)
            {
                return res.Error;
            }

            _components.Add(res.Value!);
        }

        return Result.Success();
    }
}
