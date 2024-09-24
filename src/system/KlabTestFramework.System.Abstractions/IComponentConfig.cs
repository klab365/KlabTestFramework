using System.Collections.Generic;
using System.Linq;
using KlabTestFramework.Shared.Parameters;

namespace KlabTestFramework.System.Abstractions;

public interface IComponentConfig
{
    string Id { get; set; }

    bool IsEnabled { get; set; }

    bool HasError { get; set; }

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
