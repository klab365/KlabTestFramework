// See https://aka.ms/new-console-template for more information
using System;
using KlabTestFramework.Shared.Parameters;
using KlabTestFramework.System.Lib.Infrastructure;
using KlabTestFramework.System.Lib.Specifications;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

IHostBuilder builder = Host.CreateDefaultBuilder(args);
builder.ConfigureServices(services =>
{
    services.AddTransient<IComponentRepository, ComponentTomlRepository>();
    services.UseParameters();
});
IHost host = builder.Build();

const string path = "sample.toml";
IComponentRepository repository = host.Services.GetRequiredService<IComponentRepository>();
ComponentData[] components = await repository.GetComponentAsync(path);

foreach (ComponentData component in components)
{
    Console.WriteLine($"{component.Name}");

    if (component.Children?.Count > 0)
    {
        foreach (ComponentData child in component.Children)
        {
            Console.WriteLine($"  {child.Name}");
        }
    }

}

