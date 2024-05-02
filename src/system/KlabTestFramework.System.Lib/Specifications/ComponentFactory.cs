using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Klab.Toolkit.Results;
using KlabTestFramework.System.Abstractions;

namespace KlabTestFramework.System.Lib.Specifications;

internal sealed class ComponentFactory
{
    private readonly IEnumerable<ComponentSpecification>? _specifications;

    public ComponentFactory(IEnumerable<ComponentSpecification>? specifications)
    {
        _specifications = specifications;
    }

    public async Task<Result<IComponent>> CreateComponentAsync(ComponentData componentData)
    {
        if (_specifications is null)
        {
            return SystemSpecificationErrors.NoComponentSpecifications;
        }

        ComponentSpecification? specification = _specifications.FirstOrDefault(s => s.Type == componentData.Type);
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

        //FIXME: The cast not work yet...
        Result<IComponent> resCreation = (Result<IComponent>)component;
        return resCreation;
    }
}
