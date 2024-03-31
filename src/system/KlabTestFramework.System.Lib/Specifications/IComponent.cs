using System.Collections.Generic;
using System.Threading.Tasks;

namespace KlabTestFramework.System.Lib.Specifications;

public interface IComponent
{
    IEnumerable<IComponent> Children { get; }

    Task ResetAsync();
}

public interface IComponent<TConfig> : IComponent where TConfig : IComponentConfig
{
    TConfig Config { get; }

    Task InitializeAsync(TConfig config);
}
