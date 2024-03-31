using System;
using System.Diagnostics;
using System.Threading.Tasks;
using KlabTestFramework.Shared.Parameters.Types;
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
        const string subName = "sub1";

        // create workflow programmatically
        IWorkflowEditor workflowEditor = services.GetRequiredService<IWorkflowEditor>();
        workflowEditor.ConfigureMetadata(m => m.Description = "My first workflow");
        workflowEditor.IncludeSubworkflow(subName, await CreateSubworkflow1(services));
        workflowEditor.AddStepToLastPosition<WaitStep>(s => s.Time.ChangetToVariable("time"));
        workflowEditor.AddStepToLastPosition<SubworkflowStep>(s =>
        {
            s.SelectSubworkflow(subName);
            s.ReplaceArgumentValue("myVariable", "00:00:10");
        });

        // run workflow
        Stopwatch stopwatch = Stopwatch.StartNew();
        IWorkflowRunner workflowEngine = services.GetRequiredService<IWorkflowRunner>();
        Klab.Toolkit.Results.Result<IWorkflow> workflowResult = await workflowEditor.BuildWorkflowAsync();
        if (workflowResult.IsFailure)
        {
            Console.WriteLine($"Workflow is invalid: {workflowResult.Error}");
            return;
        }

        IWorkflowContext context = services.GetRequiredService<IWorkflowContext>();
        IWorkflow workflow = workflowResult.Value!;
        await workflowEngine.RunAsync(workflow, context);
        stopwatch.Stop();
        Console.WriteLine($"Workflow finished in {stopwatch.ElapsedMilliseconds} ms");
    }

    public static async Task<IWorkflow> CreateSubworkflow1(IServiceProvider services)
    {
        IWorkflowEditor workflowEditor = services.GetRequiredService<IWorkflowEditor>();
        workflowEditor.CreateNewWorkflow();
        workflowEditor.ConfigureMetadata(m => m.Description = "My first subworkflow");
        workflowEditor.AddVariable<TimeParameter>("myVariable", "sec", VariableType.Argument, p => p.SetValue(TimeSpan.FromSeconds(3)));
        workflowEditor.AddStepToLastPosition<WaitStep>(s =>
        {
            s.Time.ChangetToVariable("myVariable");
            s.Id = StepId.Create("wait1");
        });
        return (await workflowEditor.BuildWorkflowAsync()).Value!;
    }
}
