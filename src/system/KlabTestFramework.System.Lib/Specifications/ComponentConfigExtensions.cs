using System.Collections.Generic;
using System.Linq;
using Klab.Toolkit.Results;
using KlabTestFramework.Shared.Parameters;
using KlabTestFramework.System.Abstractions;

namespace KlabTestFramework.System.Lib.Specifications;

internal static class ComponentConfigExtensions
{
    public static ComponentData ToData(this IComponentConfig config)
    {
        ComponentData componentData = new()
        {
            Id = config.Id.Value,
            Name = config.Name,
            ImagePath = config.ImagePath,
            Type = config.GetType().Name,
        };

        foreach (IParameterType parameter in config.Parameters)
        {
            componentData.Parameters.Add(parameter.Name, parameter.AsString());
        }

        foreach (IComponentConfig child in config.Children)
        {
            componentData.Children ??= [];
            componentData.Children.Add(child.ToData());
        }

        return componentData;
    }

    public static Result FromData(this IComponentConfig config, ComponentData data)
    {
        config.Id = ComponentId.Create(data.Id);
        config.Name = data.Name;
        config.ImagePath = data.ImagePath;

        foreach (IParameterType parameter in config.Parameters)
        {
            KeyValuePair<string, string> foundParameter = data.Parameters.First(p => p.Key == parameter.Name);
            if (foundParameter.Value is null)
            {
                return SystemSpecificationErrors.ParameterNotFound;
            }

            parameter.FromString(foundParameter.Value);
        }

        foreach (IComponentConfig child in config.Children)
        {
            ComponentData? childData = data.Children?.Find(c => c.Id == child.Id.Value);
            if (childData is null)
            {
                return new Error(2, "Child not found");
            }

            Result result = child.FromData(childData);
            if (result.IsFailure)
            {
                return result;
            }
        }

        return Result.Success();
    }
}
