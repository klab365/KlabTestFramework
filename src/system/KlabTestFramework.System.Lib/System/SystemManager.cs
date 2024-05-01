using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Klab.Toolkit.Results;
using KlabTestFramework.System.Lib.Specifications;

namespace KlabTestFramework.System.Lib.System;

public class SystemManager
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
        List<IComponent> components = new();
        foreach (ComponentData data in componentData)
        {
            Result<IComponent> res = await _componentFactory.CreateComponentAsync(data);
            if (res.IsFailure)
            {
                return res.Error;
            }

            components.Add(res.Value!);
        }

        _components.Clear();
        _components.AddRange(components);
        return Result.Success();
    }

    public async Task<Result> ResetAsync()
    {
        foreach (IComponent component in _components)
        {
            await component.ResetAsync();
        }

        return Result.Success();
    }

    public Result<TComponent> GetComponentById<TComponent>(string id) where TComponent : IComponent
    {
        IComponent? component = _components.Find(c => c.GetConfig().Id.Value == id);
        if (component is null)
        {
            return SystemManagerErrors.ComponentNotFound;
        }

        if (component is not TComponent tComponent)
        {
            return SystemManagerErrors.ComponentTypeMismatch;
        }

        return tComponent;
    }

    public Result<TComponent[]> GetAllComponents<TComponent>() where TComponent : IComponent
    {
        TComponent[] components = _components.OfType<TComponent>().ToArray();
        return components;
    }
}

public record SystemManagerOptions
{
    public string Path { get; set; } = string.Empty;
}
