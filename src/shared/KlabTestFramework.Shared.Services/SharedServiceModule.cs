using Microsoft.Extensions.DependencyInjection;

namespace KlabTestFramework.Shared.Services;

public static class SharedServiceModule
{
    public static void UseSharedServices(this IServiceCollection services)
    {
        services.AddSingleton<IThreadProvider, ThreadProvider>();
    }
}
