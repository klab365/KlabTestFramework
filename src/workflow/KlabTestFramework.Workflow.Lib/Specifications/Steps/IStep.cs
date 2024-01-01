using System.Collections.Generic;

namespace KlabTestFramework.Workflow.Lib.Specifications;

/// <summary>
/// Represents a step in a workflow.
/// </summary>
public interface IStep
{
    /// <summary>
    /// Get the parameters of the step.
    /// </summary>
    /// <returns></returns>
    IEnumerable<IParameter> GetParameters();
}
