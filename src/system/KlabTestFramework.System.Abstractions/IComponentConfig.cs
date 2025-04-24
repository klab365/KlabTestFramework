using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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

    Task<ComponentConfigValdationResult> ValidateComponentAsync(CancellationToken cancellationToken = default);
}
