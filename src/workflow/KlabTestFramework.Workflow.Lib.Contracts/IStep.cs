using System.Collections.Generic;

namespace KlabTestFramework.Workflow.Lib.Contracts;

/// <summary>
/// Represents a step in a workflow.
/// </summary>
public interface IStep
{
    /// <summary>
    /// Gets the parameters associated with the step.
    /// </summary>
    /// <returns>An enumerable collection of <see cref="IParameter"/> objects.</returns>
    IEnumerable<IParameter> GetParameters();
}
