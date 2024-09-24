using System.Collections.Generic;
using System.Threading.Tasks;
using Klab.Toolkit.Results;
using KlabTestFramework.Shared.Parameters;
using KlabTestFramework.Shared.Parameters.Types;
using KlabTestFramework.System.Abstractions;

namespace KlabTestFramework.System.Types.Dummy;

public sealed class DummyChildComponent : IComponent<DummyChildComponentConfig>
{
    public IEnumerable<IComponent> Children => [];

    public DummyChildComponentConfig Config { get; set; } = new();

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public Task<Result> InitializeAsync()
    {
        return Task.FromResult(Result.Success());
    }

    public Task<Result> ResetAsync()
    {
        return Task.FromResult(Result.Success());
    }
}

public class DummyChildComponentConfig : IComponentConfig
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = "Dummy Child Component";
    public string ImagePath { get; set; } = string.Empty;

    public bool IsEnabled { get; set; } = true;
    public bool HasError { get; set; }
    public StringParameter DummyParameter { get; } = new() { Name = "DummyParameter" };

    public IEnumerable<IParameterType> Parameters => [DummyParameter];

    public IEnumerable<IComponentConfig> Children => [];
}
