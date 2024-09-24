using System;
using System.Collections.Generic;
using System.Linq;
using Klab.Toolkit.Results;
using KlabTestFramework.Shared.Parameters;
using KlabTestFramework.System.Abstractions;

namespace KlabTestFramework.System.Lib.Specifications;

/// <summary>
/// Extension Class for <see cref="IComponentConfig"/>
/// </summary>
internal static class ComponentConfigExtensions
{
    public static ComponentData ToData(this IComponentConfig config)
    {
        ComponentData componentData = new()
        {
            Id = config.Id,
            IsEnabled = config.IsEnabled,
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
        config.Id = data.Id;
        config.IsEnabled = data.IsEnabled;
        config.Name = data.Name;
        config.ImagePath = data.ImagePath;

        // handle parameters
        foreach (IParameterType parameter in config.Parameters)
        {
            KeyValuePair<string, string> foundParameter = data.Parameters.FirstOrDefault(p => string.Equals(p.Key, parameter.Name, StringComparison.OrdinalIgnoreCase));
            if (foundParameter.Value is null)
            {
                return Result.Failure(SystemErrors.ParameterNotFound);
            }

            parameter.FromString(foundParameter.Value);
        }

        return Result.Success();
    }
}
