using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Klab.Toolkit.Results;
using KlabTestFramework.Workflow.Lib.BuildInSteps;
using KlabTestFramework.Workflow.Lib.Editor;
using KlabTestFramework.Workflow.Lib.Runner;
using KlabTestFramework.Workflow.Lib.Specifications;
using Microsoft.Extensions.DependencyInjection;

namespace WorkflowConsoleExample;

public class RunWorkflowFromFileExample : IRunExample
{
    public async Task Run(IServiceProvider services)
    {
        Stopwatch watch = Stopwatch.StartNew();
        const string workflowPath = "workflow.json";

        // write to worklfow.json
        IWorkflowEditor workflowEditor = services.GetRequiredService<IWorkflowEditor>();
        workflowEditor.CreateNewWorkflow();
        workflowEditor.ConfigureMetadata(m => m.Description = "My first workflow from a file");
        workflowEditor.ConfigureMetadata(m => m.Author = "Klab");
        workflowEditor.AddVariable<IntParameter>("myVariable", v => v.SetValue(5));
        workflowEditor.AddVariable<TimeParameter>("myVariable2", v => v.SetValue(TimeSpan.FromSeconds(5)));
        workflowEditor.AddStep<WaitStep>(s => s.Time.Content.SetValue(TimeSpan.FromSeconds(5)));
        workflowEditor.AddStep<WaitStep>(s => s.Time.ChangetToVariable("myVariable"));

        Workflow workflow = workflowEditor.BuildWorkflow().Value!;
        await workflowEditor.SaveWorkflowAsync(workflowPath, workflow);
        watch.Stop();
        Console.WriteLine($"Workflow saved to {workflowPath} in {watch.Elapsed.TotalMilliseconds}ms");

        // run workflow.json
        Result<Workflow> readWorkflow = await workflowEditor.LoadWorkflowFromFileAsync(workflowPath);
        if (readWorkflow.IsFailure)
        {
            Console.WriteLine($"Failed to load workflow from {workflowPath}");
            return;
        }

        IWorkflowRunner runner = services.GetRequiredService<IWorkflowRunner>();
        await runner.RunAsync(readWorkflow.Value!);
    }
}
