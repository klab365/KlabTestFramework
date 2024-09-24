// See https://aka.ms/new-console-template for more information
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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
            services.UseSystemLib();
            services.UseDummyComponents();
            services.UseParameters();
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

        Print(0, systemManager.Components);

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
