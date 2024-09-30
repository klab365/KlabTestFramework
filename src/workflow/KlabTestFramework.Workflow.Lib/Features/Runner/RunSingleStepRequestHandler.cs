using System.Threading;
using System.Threading.Tasks;
using Klab.Toolkit.Event;
using Klab.Toolkit.Results;
using KlabTestFramework.Workflow.Lib.Specifications;

namespace KlabTestFramework.Workflow.Lib.Features.Runner;


internal class RunSingleStepRequestHandler : IRequestHandler<RunSingleStepRequest, WorkflowStepStatusEvent>
{
    private readonly StepFactory _stepFactory;

    public RunSingleStepRequestHandler(StepFactory stepFactory)
    {
        _stepFactory = stepFactory;
    }

    public async Task<Result<WorkflowStepStatusEvent>> HandleAsync(RunSingleStepRequest request, CancellationToken cancellationToken)
    {
        IStepHandler stepHandler = _stepFactory.CreateStepHandler(request.Step);
        Result res = await stepHandler.HandleAsync(request.Step, request.Context, cancellationToken);
        return Result.Success(new WorkflowStepStatusEvent(request.Step, res.IsSuccess ? StepStatus.Completed : StepStatus.Idle));
    }
}

public record RunSingleStepRequest(IStep Step, WorkflowContext Context) : IRequest<WorkflowStepStatusEvent>;

/// <summary>
/// Provides data for the StepStatusChanged event.
/// </summary>
public record WorkflowStepStatusEvent(IStep Step, StepStatus Status);

/// <summary>
/// Represents the status of a workflow step.
/// </summary>
public enum StepStatus
{
    ///
    /// Idle,
    Idle,

    /// <summary>
    /// The step is running.
    /// </summary>
    Running,

    /// <summary>
    /// The step is completed.
    /// </summary>
    Completed
}
