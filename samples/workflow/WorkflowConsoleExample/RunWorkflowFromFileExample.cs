using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Klab.Toolkit.Results;
using KlabTestFramework.Workflow.Lib.Editor;
using KlabTestFramework.Workflow.Lib.Runner;
using KlabTestFramework.Workflow.Lib.Specifications;
using Microsoft.Extensions.DependencyInjection;

namespace WorkflowConsoleExample;

public class RunWorkflowFromFileExample : IRunExample
{
    public async Task Run(IServiceProvider services)
    {
        const string workflowName = "workflow.json";
        string workflowPath = Assembly.GetExecutingAssembly().Location;
        workflowPath = Path.Join(Path.GetDirectoryName(workflowPath)!, workflowName);
        Console.WriteLine($"Running workflow from {workflowName} in {workflowPath}");
        IWorkflowEditor workflowEditor = services.GetRequiredService<IWorkflowEditor>();

        // run workflow.json
        Stopwatch watch = Stopwatch.StartNew();
        watch.Restart();
        Result resultReadWorkflow = await workflowEditor.LoadWorkflowFromFileAsync(workflowPath);
        if (resultReadWorkflow.IsFailure)
        {
            Console.WriteLine($"Failed to load workflow from {workflowName}");
            return;
        }

        IWorkflowRunner runner = services.GetRequiredService<IWorkflowRunner>();
        IWorkflowContext context = services.GetRequiredService<IWorkflowContext>();
        Result<IWorkflow> workflow = await workflowEditor.BuildWorkflowAsync();
        await runner.RunAsync(workflow.Value!, context);
        watch.Stop();
        Console.WriteLine($"Workflow executed in {watch.Elapsed.TotalMilliseconds}ms");
    }
}
