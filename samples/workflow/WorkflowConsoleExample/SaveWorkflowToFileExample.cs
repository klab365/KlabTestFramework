using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Klab.Toolkit.Results;
using KlabTestFramework.Shared.Parameters.Types;
using KlabTestFramework.Workflow.Lib.BuiltIn;
using KlabTestFramework.Workflow.Lib.Editor;
using KlabTestFramework.Workflow.Lib.Specifications;
using Microsoft.Extensions.DependencyInjection;

namespace WorkflowConsoleExample;

public class SaveWorkflowToFileExample : IRunExample
{
    public async Task Run(IServiceProvider services)
    {
        const string workflowPath = "workflow.json";
        const string subName = "sub1";
        Stopwatch watch = Stopwatch.StartNew();

        // write to worklfow.json
        IWorkflowEditor workflowEditor = services.GetRequiredService<IWorkflowEditor>();
        workflowEditor.CreateNewWorkflow();
        workflowEditor.ConfigureMetadata(m => m.Description = "My first workflow from a file");
        workflowEditor.IncludeSubworkflow(subName, await CreateSubworkflow1(services));
        workflowEditor.AddStepToLastPosition<SubworkflowStep>(s =>
        {
            s.SelectedSubworkflow.Content.Value.SetValue(subName);
            s.ReplaceArgumentValue("myVariable", "00:00:10");
        });
        workflowEditor.AddStepToLastPosition<SubworkflowStep>(s =>
        {
            s.SelectedSubworkflow.Content.Value.SetValue(subName);
            s.ReplaceArgumentValue("myVariable", "00:00:06");
        });
        workflowEditor.AddStepToLastPosition<SubworkflowStep>(s =>
        {
            s.SelectedSubworkflow.Content.Value.SetValue(subName);
            s.ReplaceArgumentValue("myVariable", "00:00:03");
        });

        await workflowEditor.SaveWorkflowAsync(workflowPath);
        watch.Stop();
        Console.WriteLine($"Workflow saved to {workflowPath} in {watch.Elapsed.TotalMilliseconds}ms");
    }

    public static async Task<IWorkflow> CreateSubworkflow1(IServiceProvider services)
    {
        IWorkflowEditor workflowEditor = services.GetRequiredService<IWorkflowEditor>();
        workflowEditor.CreateNewWorkflow();
        workflowEditor.ConfigureMetadata(m => m.Description = "My first subworkflow");
        workflowEditor.AddVariable<TimeParameter>("myVariable", "sec", VariableType.Argument, p => p.SetValue(TimeSpan.FromSeconds(3)));
        workflowEditor.AddStepToLastPosition<WaitStep>(s => s.Time.ChangetToVariable("myVariable"));
        Result<IWorkflow> subworkflow = await workflowEditor.BuildWorkflowAsync();
        return subworkflow.Value!;
    }
}
