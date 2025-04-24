using System;
using Klab.Toolkit.Event;
using KlabTestFramework.Shared.Parameters;
using KlabTestFramework.Shared.Services;
using Microsoft.Extensions.DependencyInjection;

namespace KlabTestFramework.Workflow.Lib.Tests;

public static class ServiceProviderTestHelper
{
    public static ServiceProvider GetServiceProvider(Action<IServiceCollection>? configure = null)
    {
        IServiceCollection services = new ServiceCollection();
        services.UseWorkflowLib(config => config.AddStepType<MockStep, MockStepHandler>());
        services.UseEventModule();
        services.AddLogging();
        configure?.Invoke(services);

        services.UseParameters();
        services.UseSharedServices();

        return services.BuildServiceProvider();
    }
}
