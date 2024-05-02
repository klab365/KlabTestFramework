// See https://aka.ms/new-console-template for more information
using System;
using System.Threading;
using Klab.Toolkit.Results;
using KlabTestFramework.Shared.Parameters;
using KlabTestFramework.System.Abstractions;
using KlabTestFramework.System.Lib;
using KlabTestFramework.System.Types.Dummy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

IHostBuilder builder = Host.CreateDefaultBuilder(args);
builder.ConfigureServices(services =>
{
    services.UseSystemLib(cfg =>
    {
        cfg.Submodules.Add(new DummySystemModule());
    });
    services.UseParameters();
});
IHost host = builder.Build();

// start...
const string path = "sample.toml";
ISystemManager systemManager = host.Services.GetRequiredService<ISystemManager>();
Result res = await systemManager.InitializeAsync(path, CancellationToken.None);
if (res.IsFailure)
{
    Console.WriteLine(res.Error);
    return;
}

foreach (IComponent component in systemManager.Components)
{
    Console.WriteLine($"{component.GetConfig().Name}");
}

