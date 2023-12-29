using System;
using System.Diagnostics;
using KlabTestFramework.Workflow.Lib;
using KlabTestFramework.Workflow.Lib.Specifications;
using KlabTestFramework.Workflow.Lib.Types;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

IHostBuilder builder = Host.CreateDefaultBuilder(args);
builder.ConfigureServices(services => services.UseWorkflowLib());
IHost host = builder.Build();

// create workflow programmatically
Random random = new();
Workflow workflow = host.Services.GetRequiredService<Workflow>();
workflow.Description = "My first workflow";
for (int i = 0; i < 1; i++)
{
    workflow.AddStep<WaitStep>(s => s.Time = TimeSpan.FromSeconds(random.Next(1, 4)));
}


// run workflow
Stopwatch stopwatch = Stopwatch.StartNew();
IWorkflowRunner workflowEngine = host.Services.GetRequiredService<IWorkflowRunner>();
await workflowEngine.RunAsync(workflow);
stopwatch.Stop();
Console.WriteLine($"Workflow finished in {stopwatch.ElapsedMilliseconds} ms");

