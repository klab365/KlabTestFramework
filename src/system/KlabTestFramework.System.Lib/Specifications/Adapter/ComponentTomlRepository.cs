
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Tomlyn;
using Tomlyn.Model;

namespace KlabTestFramework.System.Lib.Specifications.Adapter;

public class ComponentTomlRepository : IComponentRepository
{
    public async Task<ComponentData[]> GetComponentAsync(string path)
    {
        using StreamReader reader = new(path);
        string content = await reader.ReadToEndAsync();

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
        ComponentData component = new();
        component.Id = (string)table["id"];
        component.Name = (string)table["name"];
        component.Type = (string)table["type"];

        if (table.TryGetValue("parameters", out object? parameters) && parameters is TomlArray parametersArray)
        {
            foreach (object? parameterItem in parametersArray)
            {
                if (parameterItem is not TomlTable parameterTable)
                {
                    continue;
                }

                ParameterData parameterData = new()
                {
                    Name = (string)parameterTable["name"],
                    Value = (string)parameterTable["value"]
                };
                component.Parameters.Add(parameterData);
            }
        }

        return component;
    }
}


