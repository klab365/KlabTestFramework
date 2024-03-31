// See https://aka.ms/new-console-template for more information
using System;
using System.Linq;
using KlabTestFramework.Shared.Parameters;
using KlabTestFramework.System.Lib.Specifications;
using KlabTestFramework.System.Lib.Specifications.Adapter;
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

Console.WriteLine(string.Join(Environment.NewLine, components.Select(c => c.Name)));




