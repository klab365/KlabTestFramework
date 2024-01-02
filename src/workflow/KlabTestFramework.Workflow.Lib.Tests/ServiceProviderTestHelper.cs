using System;
using Microsoft.Extensions.DependencyInjection;

namespace KlabTestFramework.Workflow.Lib.Tests;

public static class ServiceProviderTestHelper
{
    public static ServiceProvider GetServiceProvider(Action<IServiceCollection>? configure = null)
    {
        IServiceCollection services = new ServiceCollection();
        services.UseWorkflowLib(config => config.AddStepType<MockStep, MockStepHandler>());
        configure?.Invoke(services);

        return services.BuildServiceProvider();
    }
}
