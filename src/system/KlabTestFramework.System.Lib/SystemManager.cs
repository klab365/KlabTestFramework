using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Klab.Toolkit.Results;
using KlabTestFramework.System.Abstractions;
using KlabTestFramework.System.Lib.Specifications;

namespace KlabTestFramework.System.Lib;

internal sealed class SystemManager : ISystemManager
{
    private readonly IComponentRepository _repository;
    private readonly ComponentFactory _componentFactory;
    private readonly List<IComponent> _components = new();

    public IEnumerable<IComponent> Components => _components;

    public IEnumerable<IComponent> FlattenComponents => _components.SelectMany(c => c.Children);

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
            return Result.Failure(SystemErrors.Cancled);
        }

        if (string.IsNullOrWhiteSpace(path))
        {
            return Result.Failure(SystemErrors.PathIsRequired);
        }

        _components.Clear();
        ComponentData[] componentData = await _repository.GetComponentAsync(path, cancellationToken);
        foreach (ComponentData data in componentData)
        {
            Result<IComponent> res = _componentFactory.CreateComponent(data);
            if (res.IsFailure)
            {
                return Result.Failure(res.Error);
            }

            _components.Add(res.Value!);
        }

        // validate components
        Result resValidation = ValidateComponents(FlattenComponents);
        if (resValidation.IsFailure)
        {
            return Result.Failure(resValidation.Error);
        }

        // initialization of components (first parent, then children)
        foreach (IComponent component in _components)
        {
            Result res = await component.InitializeAsync();
            if (res.IsFailure)
            {
                return res;
            }

            foreach (IComponent child in component.Children)
            {
                res = await child.InitializeAsync();
                if (res.IsFailure)
                {
                    return res;
                }
            }
        }

        return Result.Success();
    }

    public async ValueTask DisposeAsync()
    {
        foreach (IComponent component in _components)
        {
            await component.DisposeAsync();
        }
    }

    public Task<Result<TComponent>> GetValidComponentAsync<TComponent>(string id, CancellationToken cancellationToken = default) where TComponent : IComponent
    {
        IComponent? component = FlattenComponents.FirstOrDefault(c => c.GetConfig().Id == id);
        if (component == null)
        {
            Result<TComponent> res = Result.Failure<TComponent>(SystemErrors.ComponentNotFound(id));
            return Task.FromResult(res);
        }

        if (component is not TComponent tComponent)
        {
            Result<TComponent> res = Result.Failure<TComponent>(SystemErrors.ComponentTypeMismatch);
            return Task.FromResult(res);
        }

        IComponentConfig config = component.GetConfig();
        if (!config.IsEnabled)
        {
            Result<TComponent> res = Result.Failure<TComponent>(SystemErrors.ComponentNotEnabled(config.Id));
            return Task.FromResult(res);
        }

        if (config.HasError)
        {
            Result<TComponent> res = Result.Failure<TComponent>(SystemErrors.ComponentHasError(config.Id));
            return Task.FromResult(res);
        }

        return Task.FromResult(Result.Success(tComponent));
    }

    public Task<Result<IEnumerable<TComponent>>> GetAllComponentsAsync<TComponent>(CancellationToken cancellationToken = default) where TComponent : IComponent
    {
        IEnumerable<TComponent> components = FlattenComponents.OfType<TComponent>();
        return Task.FromResult(Result.Success(components));
    }

    public Task<Result<IEnumerable<IComponent>>> GetAllComponentsAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result.Success(FlattenComponents));
    }

    private static Result ValidateComponents(IEnumerable<IComponent> flattenComponents)
    {
        Result res = ValidateUniqueIds(flattenComponents);
        if (res.IsFailure)
        {
            return res;
        }

        return Result.Success();
    }

    private static Result ValidateUniqueIds(IEnumerable<IComponent> components)
    {
        HashSet<string> ids = [];
        foreach (IComponent component in components)
        {
            string id = component.GetConfig().Id;
            if (ids.Contains(id))
            {
                return Result.Failure(SystemErrors.DuplicateComponentId(id));
            }
            ids.Add(id);
        }

        return Result.Success();
    }
}
