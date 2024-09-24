using System;
using System.Collections.Generic;
using System.Linq;
using Klab.Toolkit.Results;
using KlabTestFramework.System.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace KlabTestFramework.System.Lib.Specifications;

internal sealed class ComponentFactory : IComponentFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly List<ComponentSpecification> _specifications;

    public ComponentFactory(IServiceProvider serviceProvider, IEnumerable<ComponentSpecification> specifications)
    {
        _serviceProvider = serviceProvider;
        _specifications = specifications.ToList();
    }

    public TComponent CreateComponent<TComponent>() where TComponent : IComponent
    {
        Type componentType = typeof(TComponent);

        ComponentSpecification? specification = _specifications.Find(s => s.ComponentType == componentType);
        if (specification is null)
        {
            throw new InvalidOperationException($"Component type '{componentType}' not found");
        }

        return _serviceProvider.GetRequiredService<TComponent>();
    }

    public Result<IComponent> CreateComponent(ComponentData componentData)
    {
        ComponentSpecification? specification = _specifications.Find(s => s.TypeKey == componentData.Type);
        if (specification is null)
        {
            return Result.Failure<IComponent>(SystemErrors.ComponentNotFound(componentData.Id));
        }

        // handle parent...
        Result<IComponent> parentRes = CreateComponent(specification, componentData);
        if (parentRes.IsFailure)
        {
            return parentRes;
        }

        // handle children...(weird solution...)
        // children are already created in parent component, so we need to update config
        if (parentRes.Value!.Children.Count() != componentData.Children.Count)
        {
            return Result.Failure<IComponent>(SystemErrors.ChildrenNotMatch(parentRes.Value.GetConfig().Id));
        }
        for (int i = 0; i < parentRes.Value.Children.Count(); i++)
        {
            parentRes.Value.Children.ElementAt(i).GetConfig().FromData(componentData.Children[i]);
        }

        return Result.Success(parentRes.Value);
    }

    private Result<IComponent> CreateComponent(ComponentSpecification specification, ComponentData componentData)
    {
        IComponentConfig config = (IComponentConfig)_serviceProvider.GetRequiredService(specification.ConfigType);
        Result res = config.FromData(componentData);
        if (res.IsFailure)
        {
            return Result.Failure<IComponent>(res.Error);
        }

        IComponent component = (IComponent)_serviceProvider.GetRequiredService(specification.ComponentType);
        component.SetConfig(config);

        return Result.Success(component);
    }
}
