// See https://aka.ms/new-console-template for more information
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Klab.Toolkit.Event;
using Klab.Toolkit.Results;
using KlabTestFramework.Shared.Parameters;
using KlabTestFramework.System.Abstractions;
using KlabTestFramework.System.Lib;
using KlabTestFramework.System.Types.Dummy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace KlabTestFramework.SystemConsoleExample;

internal sealed class Program
{
    private Program()
    {
    }

    private static async Task Main(string[] args)
    {
        IHostBuilder builder = Host.CreateDefaultBuilder(args);
        builder.ConfigureServices(services =>
        {
            services.UseParameters();
            services.UseEventModule();
            services.UseSystemLib(config =>
            {
                config
                    .RegisterComponent(ComponentSpecification.Create<DummyComponentConfig, DummyComponent>())
                    .RegisterComponent(ComponentSpecification.Create<DummyChildComponentConfig, DummyChildComponent>());

                config.RegisterCommunicator(CommunicatorSpecification.Create<DummyCommunicator>());
            });
        });
        IHost host = builder.Build();

        // start...
        const string fileName = "sample.toml";
        string path = Path.Combine(AppContext.BaseDirectory, fileName);
        ISystemManager systemManager = host.Services.GetRequiredService<ISystemManager>();
        Result res = await systemManager.InitializeAsync(path, CancellationToken.None);
        if (res.IsFailure)
        {
            Console.WriteLine(res.Error);
            return;
        }

        // print available component specifications
        Console.WriteLine("Available component specifications:");
        IEnumerable<ComponentSpecification> specifications = host.Services.GetRequiredService<IEnumerable<ComponentSpecification>>();
        foreach (ComponentSpecification specification in specifications)
        {
            Console.WriteLine($"Component: {specification.ComponentType.Name}");
            Console.WriteLine($"  TypeKey: {specification.TypeKey}");
            Console.WriteLine($"  ConfigType: {specification.ConfigType.Name}");
        }
        Console.WriteLine();

        Console.WriteLine("Components:");
        Result<IEnumerable<IComponent>> components = await systemManager.GetAllComponentsAsync();
        if (components.IsSuccess)
        {
            Print(0, components.Value);
        }

        await systemManager.DisposeAsync();
    }

    private static void Print(int intension, IEnumerable<IComponent> components)
    {
        foreach (IComponent component in components)
        {
            Console.WriteLine($"{new string(' ', intension)}{component.GetConfig().Id}");
            foreach (IParameterType parameter in component.GetConfig().Parameters)
            {
                Console.WriteLine($"{new string(' ', intension)}  {parameter.Name}: {parameter.AsString()}");
            }

            Print(intension + 2, component.Children);
        }
    }
}
