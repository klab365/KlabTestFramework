using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Klab.Toolkit.Results;
using KlabTestFramework.Workflow.Lib.BuiltIn;
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
        workflowEditor.AddVariable<IntParameter>("myVariable", "sec", VariableType.Constant, v => v.SetValue(5));
        workflowEditor.AddVariable<TimeParameter>("myVariable2", "sec", VariableType.Constant, p => p.SetValue(TimeSpan.FromSeconds(5)));
        workflowEditor.AddStep<WaitStep>(s => s.Time.Content.SetValue(TimeSpan.FromSeconds(5)));
        workflowEditor.AddStep<WaitStep>(s => s.Time.Content.SetValue(TimeSpan.FromSeconds(1)));

        Workflow workflow = (await workflowEditor.BuildWorkflowAsync()).Value!;
        await workflowEditor.SaveWorkflowAsync(workflowPath, workflow);
        watch.Stop();
        Console.WriteLine($"Workflow saved to {workflowPath} in {watch.Elapsed.TotalMilliseconds}ms");

        // run workflow.json
        watch.Restart();
        Result<Workflow> readWorkflow = await workflowEditor.LoadWorkflowFromFileAsync(workflowPath);
        if (readWorkflow.IsFailure)
        {
            Console.WriteLine($"Failed to load workflow from {workflowPath}");
            return;
        }

        IWorkflowRunner runner = services.GetRequiredService<IWorkflowRunner>();
        await runner.RunAsync(readWorkflow.Value!);
        watch.Stop();
        Console.WriteLine($"Workflow executed in {watch.Elapsed.TotalMilliseconds}ms");
    }
}
