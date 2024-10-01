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

    public async Task<Result> HandleAsync(SubworkflowStep step, WorkflowContext context, CancellationToken cancellationToken = default)
    {
        if (step.SelectedSubworkflow.Content.Value == SubworkflowStep.NoneSelected)
        {
            return Result.Failure(new InformativeError(string.Empty, string.Empty));
        }

        if (step.Subworkflow == null)
        {
            return Result.Failure(new InformativeError(string.Empty, string.Empty));
        }

        foreach (IStep child in step.Subworkflow.Steps)
        {
            await _eventBus.SendAsync<RunSingleStepRequest, WorkflowStepStatusEvent>(new RunSingleStepRequest(child, context), cancellationToken);
        }

        return Result.Success();
    }
}
