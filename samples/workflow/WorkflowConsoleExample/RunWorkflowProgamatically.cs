using System;
using System.Diagnostics;
using System.Threading.Tasks;
using KlabTestFramework.Workflow.Lib.BuiltIn;
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
        IWorkflowEditor workflowEditor = services.GetRequiredService<IWorkflowEditor>();
        workflowEditor.ConfigureMetadata(m => m.Description = "My first workflow");
        workflowEditor.AddStep<WaitStep>(s => s.Time.ChangetToVariable("time"));

        // run workflow
        Stopwatch stopwatch = Stopwatch.StartNew();
        IWorkflowRunner workflowEngine = services.GetRequiredService<IWorkflowRunner>();
        Klab.Toolkit.Results.Result<Workflow> workflowResult = await workflowEditor.BuildWorkflowAsync();
        if (workflowResult.IsFailure)
        {
            Console.WriteLine($"Workflow is invalid: {workflowResult.Error}");
            return;
        }

        IWorkflowContext context = services.GetRequiredService<IWorkflowContext>();
        Workflow workflow = workflowResult.Value!;
        await workflowEngine.RunAsync(workflow, context);
        stopwatch.Stop();
        Console.WriteLine($"Workflow finished in {stopwatch.ElapsedMilliseconds} ms");
    }
}
