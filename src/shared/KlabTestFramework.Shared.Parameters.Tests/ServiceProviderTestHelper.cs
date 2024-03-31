using Microsoft.Extensions.DependencyInjection;

namespace KlabTestFramework.Shared.Parameters.Tests;

public static class ServiceProviderTestHelper
{
    public static ServiceProvider CreateServiceProvider()
    {
        IServiceCollection services = new ServiceCollection();
        services.UseParameters();
        return services.BuildServiceProvider();
    }
}
