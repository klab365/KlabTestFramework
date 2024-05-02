using System.Collections.Generic;
using System.Threading.Tasks;
using Klab.Toolkit.Results;
using KlabTestFramework.System.Lib.Specifications;

namespace KlabTestFramework.System.Lib.System;

internal class SystemManager
{
    private readonly IComponentRepository _repository;
    private readonly ComponentFactory _componentFactory;
    private readonly SystemManagerOptions _options;
    private readonly List<IComponent> _components = new();

    public IEnumerable<IComponent> Components => _components;

    public SystemManager(
        SystemManagerOptions options,
        IComponentRepository repository,
        ComponentFactory componentFactory)
    {
        _repository = repository;
        _componentFactory = componentFactory;
        _options = options;
    }

    public async Task<Result> InitializeAsync()
    {
        if (string.IsNullOrWhiteSpace(_options.Path))
        {
            return SystemManagerErrors.PathIsRequired;
        }

        ComponentData[] componentData = await _repository.GetComponentAsync(_options.Path);
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

public record SystemManagerOptions
{
    public string Path { get; set; } = string.Empty;
}
