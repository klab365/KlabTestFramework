using System.Threading.Tasks;
using Klab.Toolkit.Results;
using KlabTestFramework.Workflow.Lib.Runner;

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
    Task<Result> HandleAsync(IStep step, IWorkflowContext context);
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
    Task<Result> HandleAsync(TStep step, IWorkflowContext context);

    /// <inheritdoc/>
    Task<Result> IStepHandler.HandleAsync(IStep step, IWorkflowContext context)
    {
        return HandleAsync((TStep)step, context);
    }
}
