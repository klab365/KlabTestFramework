using System.Collections.Generic;
using KlabTestFramework.Workflow.Lib.Specifications;

namespace KlabTestFramework.Workflow.Lib;

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
    /// Get the parameters of the step.
    /// </summary>
    /// <returns></returns>
    IEnumerable<ParameterContainer> GetParameters();
}

/// <summary>
/// Type for the parameter
/// </summary>
/// <param name="Key"></param>
/// <param name="Value"></param>
/// <returns></returns>
public record ParameterContainer(string Key, IParameter Value);
