using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Klab.Toolkit.Results;

namespace KlabTestFramework.System.Lib.Specifications;

public class ComponentFactory
{
    private readonly IEnumerable<ComponentFactorySpecification> _specifications;

    public ComponentFactory(IEnumerable<ComponentFactorySpecification> specifications)
    {
        _specifications = specifications;
    }

    public async Task<Result<IComponent>> CreateComponentAsync(ComponentData componentData)
    {
        ComponentFactorySpecification? specification = _specifications.FirstOrDefault(s => s.Type == componentData.Type);
        if (specification is null)
        {
            return SystemSpecificationErrors.ComponentTypeNotFound;
        }

        IComponentConfig config = specification.CreateConfig();
        Result res = config.FromData(componentData);
        if (res.IsFailure)
        {
            return res.Error;
        }

        IComponent component = specification.CreateComponent();
        await component.InitializerAsync(config);
        Result<IComponent> resCreation = (Result<IComponent>)component;
        return resCreation;
    }
}

public record ComponentFactorySpecification(
    string Type,
    Func<IComponentConfig> CreateConfig,
    Func<IComponent> CreateComponent
);
