using System;
using System.Linq;
using System.Threading.Tasks;
using Klab.Toolkit.Results;
using KlabTestFramework.Workflow.Lib.Runner;
using KlabTestFramework.Workflow.Lib.Specifications;


namespace KlabTestFramework.Workflow.Lib.BuiltIn;

public class SubworkflowStepHandler : IStepHandler<SubworkflowStep>
{
    private readonly IWorkflowRunner _workflowRunner;

    public SubworkflowStepHandler(IWorkflowRunner workflowRunner)
    {
        _workflowRunner = workflowRunner;
    }

    public async Task<Result> HandleAsync(SubworkflowStep step, IWorkflowContext context)
    {
        if (step.SelectedSubworkflow.Content.Value == SubworkflowStep.NoneSelected)
        {
            return Result.Failure(new Error(0, string.Empty));
        }

        if (step.Subworkflow == null)
        {
            return Result.Failure(new Error(0, string.Empty));
        }

        Console.WriteLine($"Executing subworkflow '{step.SelectedSubworkflow.Content.Value}'");

        IStep[] steps = step.Children.ToArray();
        IVariable[] variables = step.Subworkflow.Variables.ToArray();
        Specifications.Workflow workflow = new(steps, variables, step.Subworkflow.Subworkflows);

        await _workflowRunner.RunSubworkflowAsync(step, context);
        return Result.Success();
    }
}
