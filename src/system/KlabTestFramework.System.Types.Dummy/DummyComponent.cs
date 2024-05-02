using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KlabTestFramework.Shared.Parameters;
using KlabTestFramework.System.Abstractions;

namespace KlabTestFramework.System.Types.Dummy;

public sealed class DummyComponent : IComponent<DummyComponentConfig>
{
    public IEnumerable<IComponent> Children => Enumerable.Empty<IComponent>();

    public DummyComponentConfig Config { get; private set; } = new();

    public ValueTask DisposeAsync()
    {
        throw new global::System.NotImplementedException();
    }

    public IComponentConfig GetConfig()
    {
        return Config;
    }

    public Task InitializeAsync(DummyComponentConfig config)
    {
        Config = config;
        return Task.CompletedTask;
    }

    public Task ResetAsync()
    {
        throw new global::System.NotImplementedException();
    }
}

public class DummyComponentConfig : IComponentConfig
{
    public ComponentId Id { get; set; } = ComponentId.Empty;
    public string Name { get; set; } = string.Empty;
    public string ImagePath { get; set; } = string.Empty;

    public IEnumerable<IParameterType> Parameters => Enumerable.Empty<IParameterType>();

    public IEnumerable<IComponentConfig> Children => Enumerable.Empty<IComponentConfig>();
}
