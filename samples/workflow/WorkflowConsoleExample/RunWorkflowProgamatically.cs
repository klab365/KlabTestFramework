using System;
using System.Diagnostics;
using System.Threading.Tasks;
using KlabTestFramework.Workflow.Lib.BuildInSteps;
using KlabTestFramework.Workflow.Lib.Editor;
using KlabTestFramework.Workflow.Lib.Runner;
using KlabTestFramework.Workflow.Lib.Specifications;
using Microsoft.Extensions.DependencyInjection;

namespace WorkflowConsoleExample;

public class RunWorkflowProgamatically : IRunExample
{
    public async Task Run(IServiceProvider services)
    {
        // create workflow programmatically
        Random random = new();
        IWorkflowEditor workflowEditor = services.GetRequiredService<IWorkflowEditor>();
        workflowEditor.ConfigureMetadata(m => m.Description = "My first workflow");
        workflowEditor.AddStep<WaitStep>(s => s.Time.ChangetToVariable("time"));

        // run workflow
        Stopwatch stopwatch = Stopwatch.StartNew();
        IWorkflowRunner workflowEngine = services.GetRequiredService<IWorkflowRunner>();
        Workflow workflow = workflowEditor.BuildWorkflow().Value!;
        await workflowEngine.RunAsync(workflow);
        stopwatch.Stop();
        Console.WriteLine($"Workflow finished in {stopwatch.ElapsedMilliseconds} ms");
    }
}
