using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Klab.Toolkit.Event;
using Klab.Toolkit.Results;
using KlabTestFramework.Workflow.Lib.Features.Common;
using KlabTestFramework.Workflow.Lib.Features.Editor;
using KlabTestFramework.Workflow.Lib.Specifications;

namespace KlabTestFramework.Workflow.Lib.Features.Runner;

internal class RunWorkflowRequestHandler : IRequestHandler<RunWorkflowRequest, WorkflowResult>
{
    private readonly IEventBus _eventBus;

    public RunWorkflowRequestHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task<WorkflowResult> HandleAsync(RunWorkflowRequest request, CancellationToken cancellationToken)
    {
        Result<Specifications.Workflow> resClonedWorkflow = await _eventBus.SendAsync(new CloneWorkflowRequest(request.Workflow), cancellationToken);
        if (resClonedWorkflow.IsFailure)
        {
            return new WorkflowResult(Array.Empty<StepResult>());
        }
        Specifications.Workflow workflow = resClonedWorkflow.Value;


        IResult resReplaceVariable = await _eventBus.SendAsync(new ReplaceWorkflowWithVariablesRequest(workflow), cancellationToken);
        if (resReplaceVariable.IsFailure)
        {
            return new WorkflowResult(Array.Empty<StepResult>());
        }

        WorkflowValidatorResult resValidation = await _eventBus.SendAsync(new ValidateWorkflowRequest(workflow), cancellationToken);
        if (resValidation.IsFailure)
        {
            return new WorkflowResult(Array.Empty<StepResult>());
        }

        WorkflowResult wflResult = await HandleWorkflowAsync(workflow, request.Context, cancellationToken);
        return wflResult;
    }

    private async Task<WorkflowResult> HandleWorkflowAsync(Specifications.Workflow workflow, WorkflowContext context, CancellationToken cancellationToken)
    {
        List<StepResult> stepResults = new();
        foreach (IStep step in workflow.Steps)
        {
            StepResult res = await _eventBus.SendAsync(new RunSingleStepRequest(step, context), cancellationToken);
            stepResults.Add(res);
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
