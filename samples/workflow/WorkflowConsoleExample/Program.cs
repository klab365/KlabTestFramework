using System;
using System.Diagnostics;
using KlabTestFramework.Workflow.Lib;
using KlabTestFramework.Workflow.Lib.Runner;
using KlabTestFramework.Workflow.Lib.Specifications;
using KlabTestFramework.Workflow.Lib.Specifications.Steps.StepTypes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

IHostBuilder builder = Host.CreateDefaultBuilder(args);
builder.ConfigureServices(services => services.UseWorkflowLib());
IHost host = builder.Build();

// create workflow programmatically
Random random = new();
IWorkflowFactory workflowFactory = host.Services.GetRequiredService<IWorkflowFactory>();
Workflow workflow = workflowFactory.CreateWorkflow();
workflow.Description = "My first workflow";
for (int i = 0; i < 5; i++)
{
    workflow.AddStep<WaitStep>(s =>
    {
        s.Time.SetValue(TimeSpan.FromSeconds(random.Next(1, 4)));
    });
}


// run workflow
Stopwatch stopwatch = Stopwatch.StartNew();
IWorkflowRunner workflowEngine = host.Services.GetRequiredService<IWorkflowRunner>();
await workflowEngine.RunAsync(workflow);
stopwatch.Stop();
Console.WriteLine($"Workflow finished in {stopwatch.ElapsedMilliseconds} ms");
