using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Klab.Toolkit.Results;
using KlabTestFramework.Shared.Parameters;
using KlabTestFramework.Shared.Parameters.Types;
using KlabTestFramework.System.Abstractions;
using KlabTestFramework.System.Lib.Specifications;

namespace KlabTestFramework.System.Types.Dummy;

public sealed class DummyComponent : IComponent<DummyComponentConfig>
{
    private DummyCommunicator? _communicator;
    private readonly IComponentFactory _componentFactory;

    public IEnumerable<IComponent> Children => [Child1];

    public DummyComponentConfig Config { get; }

    public DummyChildComponent Child1 { get; }

    public DummyComponent(DummyComponentConfig config, IComponentFactory componentFactory)
    {
        Config = config;
        _componentFactory = componentFactory;
        Child1 = componentFactory.CreateComponent<DummyChildComponent>();
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public async Task<Result> InitializeAsync(CancellationToken cancellationToken = default)
    {
        _communicator = _componentFactory.CreateCommunicator<DummyCommunicator>();
        await _communicator.OpenAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> ResetAsync(CancellationToken cancellationToken = default)
    {
        if (_communicator is not null)
        {
            return await _communicator.ResetAsync(cancellationToken);
        }

        return Result.Success();
    }
}

public class DummyComponentConfig : IComponentConfig
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;
    public bool HasError { get; set; }

    public SelectableParameter<StringParameter> CommunicatorType { get; }

    public StringParameter Ip { get; } = new() { Name = "Ip" };

    public DummyChildComponentConfig Child1Config { get; } = new();

    public IEnumerable<IParameterType> Parameters => [Ip];

    public IEnumerable<IComponentConfig> Children => [Child1Config];

    public DummyComponentConfig(ParameterFactory parameterFactory)
    {
        CommunicatorType = parameterFactory.CreateParameterType<SelectableParameter<StringParameter>>();
        CommunicatorType.Name = "CommunicatorType";
        CommunicatorType.AddOptions("DummyCommunicator", "DummyCommunicator");
    }

    public Task<ComponentConfigValdationResult> ValidateComponentAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(ComponentConfigValdationResult.Success());
    }
}
