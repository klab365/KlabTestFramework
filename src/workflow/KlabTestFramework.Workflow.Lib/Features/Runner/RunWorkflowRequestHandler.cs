using System;
using System.Threading;
using System.Threading.Tasks;
using Klab.Toolkit.Event;
using Klab.Toolkit.Results;
using KlabTestFramework.Workflow.Lib.Features.Validator;
using KlabTestFramework.Workflow.Lib.Specifications;

namespace KlabTestFramework.Workflow.Lib.Features.Runner;

internal class RunWorkflowRequestHandler : IRequestHandler<RunWorkflowRequest, WorkflowResult>
{
    private readonly IEventBus _eventBus;
    private readonly IVariableReplacer _variableReplacer;

    public RunWorkflowRequestHandler(IEventBus eventBus, IVariableReplacer variableReplacer)
    {
        _eventBus = eventBus;
        _variableReplacer = variableReplacer;
    }

    public async Task<Result<WorkflowResult>> HandleAsync(RunWorkflowRequest request, CancellationToken cancellationToken)
    {
        await _variableReplacer.ReplaceVariablesWithTheParametersAsync(request.Workflow);

        Result<WorkflowValidatorResult> resValidation = await _eventBus.SendAsync<ValidateWorkflowRequest, WorkflowValidatorResult>(new ValidateWorkflowRequest(request.Workflow), cancellationToken);
        if (resValidation.IsFailure)
        {
            return Result.Failure<WorkflowResult>(resValidation.Error);
        }

        WorkflowResult wflResult = await HandleWorkflowAsync(request.Workflow, request.Context, request.Progress, cancellationToken);
        return Result.Success(wflResult);
    }

    private async Task<WorkflowResult> HandleWorkflowAsync(IWorkflow workflow, WorkflowContext context, IProgress<WorkflowStatusEvent> progress, CancellationToken cancellationToken)
    {
        progress.Report(new(WorkflowStatus.Running));


        foreach (IStep step in workflow.Steps)
        {
            await _eventBus.SendAsync<RunSingleStepRequest, WorkflowStepStatusEvent>(new RunSingleStepRequest(step, context), cancellationToken);
        }

        return new WorkflowResult(true);
    }
}

public record RunWorkflowRequest(IWorkflow Workflow, WorkflowContext Context, IProgress<WorkflowStatusEvent> Progress) : IRequest;

/// <summary>
/// Default implementation of <see cref="IWorkflowContext"/> interface.
/// </summary>
public class WorkflowContext
{
    /// <inheritdoc/>
    public IVariable[] Variables { get; set; } = Array.Empty<IVariable>();
}

public record WorkflowResult(bool IsSuccess);

public record WorkflowStatusEvent(WorkflowStatus Status);

public enum WorkflowStatus
{
    Idle,
    Running,
    Paused,
    Completed
}



