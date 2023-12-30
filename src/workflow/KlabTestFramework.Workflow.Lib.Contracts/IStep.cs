using System.Collections.Generic;

namespace KlabTestFramework.Workflow.Lib.Contracts;

/// <summary>
/// Represents a step in a workflow.
/// </summary>
public interface IStep
{
    /// <summary>
    /// Initialize the step with the given parameter data.
    /// </summary>
    /// <param name="parameterData"></param>
    void Init(IEnumerable<ParameterData> parameterData);

    /// <summary>
    /// Get the parameter data of the step.
    /// </summary>
    /// <returns></returns>
    IEnumerable<ParameterData>? GetParameterData();
}
