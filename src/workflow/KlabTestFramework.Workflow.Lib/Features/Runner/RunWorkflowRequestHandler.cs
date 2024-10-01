using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Klab.Toolkit.Event;
using Klab.Toolkit.Results;
using KlabTestFramework.Workflow.Lib.Features.Common;
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
        IResult resReplaceVariable = await _eventBus.SendAsync(new ReplaceWorkflowWithVariablesRequest(request.Workflow), cancellationToken);
        if (resReplaceVariable.IsFailure)
        {
            return Result.Failure<WorkflowResult>(resReplaceVariable.Error);
        }

        IResult<WorkflowValidatorResult> resValidation = await _eventBus.SendAsync<ValidateWorkflowRequest, WorkflowValidatorResult>(new ValidateWorkflowRequest(request.Workflow), cancellationToken);
        if (resValidation.IsFailure)
        {
            return Result.Failure<WorkflowResult>(resValidation.Error);
        }

        WorkflowResult wflResult = await HandleWorkflowAsync(request.Workflow, request.Context, cancellationToken);
        return Result.Success(wflResult);
    }

    private async Task<WorkflowResult> HandleWorkflowAsync(Specifications.Workflow workflow, WorkflowContext context, CancellationToken cancellationToken)
    {
        List<StepResult> stepResults = new();
        foreach (IStep step in workflow.Steps)
        {
            IResult<StepResult> res = await _eventBus.SendAsync<RunSingleStepRequest, StepResult>(new RunSingleStepRequest(step, context), cancellationToken);
            if (res.IsFailure)
            {
                throw new InvalidOperationException("Workflow can not run and don't return any results...");
            }

            stepResults.Add(res.Value);
        }

        return new WorkflowResult(stepResults.ToArray());
    }
}

public record RunWorkflowRequest(Specifications.Workflow Workflow, WorkflowContext Context) : IRequest<WorkflowResult>;

public record WorkflowResult(StepResult[] Results)
{
    public bool IsSuccess()
    {
        return Array.TrueForAll(Results, IsSuccess);
    }

    private static bool IsSuccess(StepResult result)
    {
        bool isSuccess = true;
        if (!result.IsSuccess)
        {
            isSuccess = false;
        }

        foreach (StepResult child in result.Children)
        {
            if (!IsSuccess(child))
            {
                isSuccess = false;
            }
        }

        return isSuccess;
    }
}
