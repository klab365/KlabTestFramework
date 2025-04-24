using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Klab.Toolkit.Results;

namespace KlabTestFramework.Workflow.Abstractions.Specifications;

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
    Task<StepResult> HandleAsync(IStep step, WorkflowContext context, CancellationToken cancellationToken = default);

    Task<StepResult> SetupAsync(IStep step, WorkflowContext context, CancellationToken cancellationToken = default);

    Task<StepResult> CleanupAsync(IStep step, WorkflowContext context, CancellationToken cancellationToken = default);
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
    Task<StepResult> HandleAsync(TStep step, WorkflowContext context, CancellationToken cancellationToken = default);

    Task<StepResult> SetupAsync(TStep step, WorkflowContext context, CancellationToken cancellationToken = default);

    Task<StepResult> CleanupAsync(TStep step, WorkflowContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// Simple wrapper to cast to the correct type and call the actual implementation.
    /// </summary>
    /// <param name="step"></param>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<StepResult> IStepHandler.HandleAsync(IStep step, WorkflowContext context, CancellationToken cancellationToken)
    {
        if (step is not TStep castedStep)
        {
            StepResult res = StepResult.Failure(step, WorkflowModuleErrors.StepNotFound);
            return Task.FromResult(res);
        }

        return HandleAsync(castedStep, context, cancellationToken);
    }

    Task<StepResult> IStepHandler.SetupAsync(IStep step, WorkflowContext context, CancellationToken cancellationToken)
    {
        if (step is not TStep castedStep)
        {
            StepResult res = StepResult.Failure(step, WorkflowModuleErrors.StepNotFound);
            return Task.FromResult(res);
        }

        return SetupAsync(castedStep, context, cancellationToken);
    }

    Task<StepResult> IStepHandler.CleanupAsync(IStep step, WorkflowContext context, CancellationToken cancellationToken)
    {
        if (step is not TStep castedStep)
        {
            StepResult res = StepResult.Failure(step, WorkflowModuleErrors.StepNotFound);
            return Task.FromResult(res);
        }

        return CleanupAsync(castedStep, context, cancellationToken);
    }
}

/// <summary>
/// Default implementation of <see cref="IWorkflowContext"/> interface.
/// </summary>
public class WorkflowContext
{
}

/// <summary>
/// Object representing the result of a step.
/// </summary>
public record StepResult
{
    public IStep Step { get; }
    public StepResult[] Children { get; }
    public Error Error { get; }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public static StepResult Success(IStep step, params StepResult[] children)
    {
        return new StepResult(step, true, Error.None(), children);
    }

    public static StepResult Failure(IStep step, Error error, params StepResult[] children)
    {
        return new StepResult(step, false, error, children);
    }

    public static StepResult Collect(IStep step, StepResult[] stepResults)
    {
        if (Array.Exists(stepResults, r => r.IsFailure))
        {
            return Failure(step, Error.Create("", ""), stepResults.ToArray());
        }
        else
        {
            return Success(step, stepResults.ToArray());
        }
    }

    private StepResult(IStep step, bool isSuccess, Error error, StepResult[] children)
    {
        Step = step;
        IsSuccess = isSuccess;
        Error = error;
        Children = children;
    }
}
