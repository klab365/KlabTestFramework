using KlabTestFramework.Workflow.Lib.Contracts;

namespace KlabTestFramework.Workflow.Lib;

/// <summary>
/// Represents a factory for creating step handlers and steps.
/// </summary>
public interface IStepFactory
{
    /// <summary>
    /// Creates a step handler wrapper for the specified step.
    /// </summary>
    /// <typeparam name="TStep">The type of the step.</typeparam>
    /// <param name="step">The step instance.</param>
    /// <returns>A step handler wrapper.</returns>
    StepHandlerWrapperBase CreateStepHandler<TStep>(TStep step) where TStep : class, IStep;

    /// <summary>
    /// Creates an instance of the specified step.
    /// </summary>
    /// <typeparam name="TStep">The type of the step.</typeparam>
    /// <returns>An instance of the step.</returns>
    IStep CreateStep<TStep>() where TStep : IStep;
}

