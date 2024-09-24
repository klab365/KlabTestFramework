
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KlabTestFramework.System.Lib.Specifications;
using Tomlyn;
using Tomlyn.Model;

namespace KlabTestFramework.System.Lib.Infrastructure;

internal sealed class ComponentTomlRepository : IComponentRepository
{
    public async Task<ComponentData[]> GetComponentAsync(string path, CancellationToken cancellationToken = default)
    {
        using StreamReader reader = new(path);
        string content = await reader.ReadToEndAsync(cancellationToken);

        List<ComponentData> components = new();
        TomlTable table = Toml.ToModel(content);

        if (table.TryGetValue("components", out object? componentsObject) && componentsObject is TomlTableArray componentsTableArray)
        {
            foreach (TomlTable item in componentsTableArray)
            {
                ComponentData componentData = HandleComponent(item);
                components.Add(componentData);
            }
        }

        return components.ToArray();
    }

    private static ComponentData HandleComponent(TomlTable table)
    {
        ComponentData component = new()
        {
            Id = (string)table["id"],
            Name = (string)table["name"],
            Type = (string)table["type"]
        };

        if (table.TryGetValue("parameters", out object? parameters) && parameters is TomlArray parametersArray)
        {
            foreach (object? parameterItem in parametersArray)
            {
                if (parameterItem is not TomlTable parameterTable)
                {
                    continue;
                }

                string? key = parameterTable.Keys.FirstOrDefault();
                if (key is null)
                {
                    continue;
                }

                string value = (string)parameterTable[key];
                component.Parameters.Add(key, value);
            }
        }

        if (table.TryGetValue("children", out object? children) && children is TomlTableArray childrenTableArray)
        {
            foreach (TomlTable child in childrenTableArray)
            {
                component.Children ??= [];
                ComponentData childComponent = HandleComponent(child);
                component.Children.Add(childComponent);
            }
        }

        return component;
    }
}


