
using KlabTestFramework.Workflow.Lib.Specifications;

namespace KlabTestFramework.Workflow.Lib.Editor;

/// <summary>
/// Represents a factory for creating step handlers and steps.
/// </summary>
public interface IStepFactory
{
    /// <summary>
    /// Creates an instance of the specified step.
    /// </summary>
    /// <typeparam name="TStep">The type of the step.</typeparam>
    /// <returns>An instance of the step.</returns>
    IStep CreateStep<TStep>() where TStep : IStep;

    /// <summary>
    /// Create an instance of the specified step by step data
    /// </summary>
    /// <param name="stepData"></param>
    /// <returns></returns>
    IStep CreateStep(StepData stepData);
}

