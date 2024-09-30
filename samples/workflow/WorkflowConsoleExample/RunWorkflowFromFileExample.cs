using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Klab.Toolkit.Event;
using Klab.Toolkit.Results;
using KlabTestFramework.Workflow.Lib.Features.Editor;
using KlabTestFramework.Workflow.Lib.Features.Runner;
using KlabTestFramework.Workflow.Lib.Specifications;
using Microsoft.Extensions.DependencyInjection;

namespace WorkflowConsoleExample;

public class RunWorkflowFromFileExample : IRunExample
{
    public async Task Run(IServiceProvider services)
    {
        const string workflowName = "workflow.yaml";
        string workflowPath = Assembly.GetExecutingAssembly().Location;
        workflowPath = Path.Join(Path.GetDirectoryName(workflowPath)!, workflowName);
        Console.WriteLine($"Running workflow from {workflowName} in {workflowPath}");
        IEventBus eventBus = services.GetRequiredService<IEventBus>();

        // run workflow.json
        Stopwatch watch = Stopwatch.StartNew();
        watch.Restart();
        Result<Workflow> resultReadWorkflow = await eventBus.SendAsync<QueryWorkflowRequest, Workflow>(new QueryWorkflowRequest(workflowPath));
        if (resultReadWorkflow.IsFailure)
        {
            Console.WriteLine($"Failed to load workflow from {workflowName}");
            return;
        }

        await eventBus.SendAsync<RunWorkflowRequest, WorkflowResult>(new RunWorkflowRequest(resultReadWorkflow.Value, new WorkflowContext()));

        watch.Stop();
        Console.WriteLine($"Workflow executed in {watch.Elapsed.TotalMilliseconds}ms");
    }
}
