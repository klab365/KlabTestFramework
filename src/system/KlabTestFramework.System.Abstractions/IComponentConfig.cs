using System.Collections.Generic;
using System.Linq;
using KlabTestFramework.Shared.Parameters;

namespace KlabTestFramework.System.Abstractions;

public interface IComponentConfig
{
    ComponentId Id { get; set; }

    string Name { get; set; }

    string ImagePath { get; set; }

    IEnumerable<IParameterType> Parameters { get; }

    IEnumerable<IComponentConfig> Children { get; }

    bool IsValid()
    {
        bool areParametersValid = Parameters.All(p => p.IsValid());
        bool areChildrenValid = Children.All(c => c.IsValid());
        return areParametersValid && areChildrenValid;
    }
}

public record ComponentId
{
    public string Value { get; }

    public static ComponentId Empty => new(string.Empty);

    public static ComponentId Create(string value) => new(value);

    private ComponentId(string value)
    {
        Value = value;
    }
}
