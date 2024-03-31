using System.Collections.Generic;
using System.Linq;

namespace KlabTestFramework.System.Lib.Specifications;

public interface IComponentConfig
{
    ComponentId Id { get; set; }

    string Name { get; set; }

    string ImagePath { get; set; }

    IEnumerable<IParameter> Parameters { get; }

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

    public ComponentId(string value)
    {
        Value = value;
    }
}
