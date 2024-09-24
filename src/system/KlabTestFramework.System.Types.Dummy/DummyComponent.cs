using System.Collections.Generic;
using System.Threading.Tasks;
using Klab.Toolkit.Results;
using KlabTestFramework.Shared.Parameters;
using KlabTestFramework.Shared.Parameters.Types;
using KlabTestFramework.System.Abstractions;
using KlabTestFramework.System.Lib.Specifications;
using Microsoft.Extensions.DependencyInjection;

namespace KlabTestFramework.System.Types.Dummy;

public sealed class DummyComponent : IComponent<DummyComponentConfig>
{
    public IEnumerable<IComponent> Children => [Child1];

    public DummyComponentConfig Config { get; set; } = new();

    public DummyChildComponent Child1 { get; }

    public DummyComponent(IComponentFactory componentFactory)
    {
        Child1 = componentFactory.CreateComponent<DummyChildComponent>();
    }

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

public class DummyComponentConfig : IComponentConfig
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ImagePath { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;
    public bool HasError { get; set; }

    public StringParameter Ip { get; } = new() { Name = "Ip" };

    public DummyChildComponentConfig Child1Config { get; } = new();

    public IEnumerable<IParameterType> Parameters => [Ip];

    public IEnumerable<IComponentConfig> Children => [Child1Config];
}

public static class DummyModule
{
    public static void UseDummyComponents(this IServiceCollection services)
    {
        services.RegisterComponent(ComponentSpecification.Create<DummyComponentConfig, DummyComponent>());
        services.RegisterComponent(ComponentSpecification.Create<DummyChildComponentConfig, DummyChildComponent>());
    }
}
