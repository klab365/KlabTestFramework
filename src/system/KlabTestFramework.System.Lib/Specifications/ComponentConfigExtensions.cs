using Klab.Toolkit.Results;

namespace KlabTestFramework.System.Lib.Specifications;

public static class ComponentConfigExtensions
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

        foreach (IParameter parameter in config.Parameters)
        {
            var parameterData = new ParameterData
            {
                Name = parameter.Name,
                Value = parameter.Value,
                Unit = parameter.Unit,
            };

            componentData.Parameters.Add(parameterData);
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
        config.Id = new ComponentId(data.Id);
        config.Name = data.Name;
        config.ImagePath = data.ImagePath;

        foreach (IParameter parameter in config.Parameters)
        {
            ParameterData? parameterData = data.Parameters.Find(p => p.Name == parameter.Name);
            if (parameterData is null)
            {
                return new Error(1, "Parameter not found");
            }

            parameter.Value = parameterData.Value;
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
