using System;
using System.Threading;
using System.Threading.Tasks;
using Klab.Toolkit.Results;

namespace KlabTestFramework.Workflow.Lib.Specifications;

/// <summary>
/// Interface for a step handler.
/// </summary>
public interface IStepHandler
{
    /// <summary>
    /// Handles the specified step in the workflow.
    /// </summary>
    /// <param name="step">The step to handle.</param>
    /// <param name="context">The context of the workflow.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<StepResults> HandleAsync(IStep step, WorkflowContext context, CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents a handler for a specific step in a workflow.
/// </summary>
/// <typeparam name="TStep">The type of step to handle.</typeparam>
public interface IStepHandler<in TStep> : IStepHandler where TStep : IStep
{
    /// <summary>
    /// Handles the specified step in the workflow.
    /// </summary>
    /// <param name="step">The step to handle.</param>
    /// <param name="context">The context of the workflow.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<StepResults> HandleAsync(TStep step, WorkflowContext context, CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    Task<StepResults> IStepHandler.HandleAsync(IStep step, WorkflowContext context, CancellationToken cancellationToken)
    {
        if (step is not TStep castedStep)
        {
            StepResults res = StepResults.Result(StepResult.Failure(step, Result.Failure(WorkflowModuleErrors.StepNotFound)));
            return Task.FromResult(res);
        }

        return HandleAsync(castedStep, context, cancellationToken);
    }
}

/// <summary>
/// Default implementation of <see cref="IWorkflowContext"/> interface.
/// </summary>
public class WorkflowContext
{
    /// <inheritdoc/>
    public IVariable[] Variables { get; set; } = [];

    public IProgress<StepFeedback> Progress { get; set; } = new Progress<StepFeedback>();
}

public record StepResults
{
    public StepResult[] Results { get; }

    public bool IsSuccess => Array.TrueForAll(Results, r => r.Result.IsSuccess);

    public static StepResults Result(params StepResult[] results)
    {
        return new StepResults(results);
    }

    private StepResults(StepResult[] results)
    {
        Results = results;
    }
}

/// <summary>
/// Object representing the result of a step.
/// </summary>
public record StepResult
{
    public IStep Step { get; }
    public Result Result { get; }
    public StepStatus Status { get;} 
    public IVariable[] OutputVariables { get; }

    public static StepResult Success(IStep step, params IVariable[] outputVariables)
    {
        return new StepResult(step, Result.Success(), StepStatus.Completed, outputVariables);
    }

    public static StepResult Failure(IStep step, Result result)
    {
        return new StepResult(step, result, StepStatus.Failed, []);
    }

    private StepResult(IStep step, Result result, StepStatus status, IVariable[] outputVariables)
    {
        Step = step;
        Result = result;
        Status = status;
        OutputVariables = outputVariables;
    }
}

public record StepFeedback
{
    public IStep Step { get; }

    public StepStatus Status { get; }

    public string Message { get; }

    public static StepFeedback Running(IStep step, string message = "")
    {
        return new StepFeedback(step, StepStatus.Running, message);
    }

    public static StepFeedback Completed(IStep step, string message = "")
    {
        return new StepFeedback(step, StepStatus.Completed, message);
    }

    public static StepFeedback Idle(IStep step, string message = "")
    {
        return new StepFeedback(step, StepStatus.Idle, message);
    }

    private StepFeedback(IStep step, StepStatus status, string message)
    {
        Step = step;
        Status = status;
        Message = message;
    }
}

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
    Completed,

    /// <summary>
    Failed,
}
