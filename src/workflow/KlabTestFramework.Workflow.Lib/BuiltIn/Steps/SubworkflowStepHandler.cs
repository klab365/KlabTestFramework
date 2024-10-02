using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Klab.Toolkit.Event;
using Klab.Toolkit.Results;
using KlabTestFramework.Workflow.Lib.Features.Runner;
using KlabTestFramework.Workflow.Lib.Specifications;


namespace KlabTestFramework.Workflow.Lib.BuiltIn;

internal class SubworkflowStepHandler : IStepHandler<SubworkflowStep>
{
    private readonly IEventBus _eventBus;

    public SubworkflowStepHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task<StepResult> HandleAsync(SubworkflowStep step, WorkflowContext context, CancellationToken cancellationToken = default)
    {
        if (step.SelectedSubworkflow.Content.Value == SubworkflowStep.NoneSelected)
        {
            return StepResult.Failure(step, new InformativeError(string.Empty, string.Empty));
        }

        if (step.Subworkflow == null)
        {
            return StepResult.Failure(step, new InformativeError(string.Empty, string.Empty));
        }

        List<StepResult> stepResults = new();
        foreach (IStep child in step.Subworkflow.Steps)
        {
            StepResult res = await _eventBus.SendAsync(new RunSingleStepRequest(child, context), cancellationToken);
            stepResults.Add(res);
        }

        return StepResult.Collect(step, stepResults.ToArray());
    }
}
