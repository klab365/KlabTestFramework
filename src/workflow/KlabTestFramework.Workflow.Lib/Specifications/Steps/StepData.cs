using System.Collections.Generic;

namespace KlabTestFramework.Workflow.Lib.Specifications;

/// <summary>
/// Data for the step
/// </summary>
public class StepData
{
    /// <summary>
    /// Id of the step
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Type of the step
    /// </summary>
    /// <value></value>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Parameters of the step, if any
    /// </summary>
    public List<ParameterData> Parameters { get; set; } = [];

    /// <summary>
    /// Children of the step, if any
    /// </summary>
    public List<StepData>? Children { get; set; }
}
