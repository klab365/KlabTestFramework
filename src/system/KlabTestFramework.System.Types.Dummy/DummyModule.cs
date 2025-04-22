using KlabTestFramework.System.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace KlabTestFramework.System.Types.Dummy;

public static class DummyModule
{
    public static IServiceCollection UseDummyComponents(this IServiceCollection services)
    {
        services.RegisterComponent(ComponentSpecification.Create<DummyComponentConfig, DummyComponent>());
        services.RegisterComponent(ComponentSpecification.Create<DummyChildComponentConfig, DummyChildComponent>());

        return services;
    }
}
