using System.Threading;
using System.Threading.Tasks;
using Klab.Toolkit.Event;
using Klab.Toolkit.Results;
using KlabTestFramework.Workflow.Lib.Features.Common;
using KlabTestFramework.Workflow.Lib.Features.Validator;
using KlabTestFramework.Workflow.Lib.Specifications;

namespace KlabTestFramework.Workflow.Lib.Features.Runner;

internal class RunWorkflowRequestHandler : IRequestHandler<RunWorkflowRequest, WorkflowResult>
{
    private readonly IEventBus _eventBus;

    public RunWorkflowRequestHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task<Result<WorkflowResult>> HandleAsync(RunWorkflowRequest request, CancellationToken cancellationToken)
    {
        Result resReplaceVariable = await _eventBus.SendAsync(new ReplaceWorkflowWithVariablesRequest(request.Workflow), cancellationToken);
        if (resReplaceVariable.IsFailure)
        {
            return Result.Failure<WorkflowResult>(resReplaceVariable.Error);
        }

        Result<WorkflowValidatorResult> resValidation = await _eventBus.SendAsync<ValidateWorkflowRequest, WorkflowValidatorResult>(new ValidateWorkflowRequest(request.Workflow), cancellationToken);
        if (resValidation.IsFailure)
        {
            return Result.Failure<WorkflowResult>(resValidation.Error);
        }

        WorkflowResult wflResult = await HandleWorkflowAsync(request.Workflow, request.Context, cancellationToken);
        return Result.Success(wflResult);
    }

    private async Task<WorkflowResult> HandleWorkflowAsync(Specifications.Workflow workflow, WorkflowContext context, CancellationToken cancellationToken)
    {
        foreach (IStep step in workflow.Steps)
        {
            await _eventBus.SendAsync<RunSingleStepRequest, StepResult>(new RunSingleStepRequest(step, context), cancellationToken);
        }

        return new WorkflowResult(true);
    }
}

public record RunWorkflowRequest(Specifications.Workflow Workflow, WorkflowContext Context) : IRequest<WorkflowResult>;

public record WorkflowResult(bool IsSuccess);

public record WorkflowStatusEvent(WorkflowStatus Status);

public enum WorkflowStatus
{
    Idle,
    Running,
    Paused,
    Completed
}
