using System.Collections.Generic;

namespace KlabTestFramework.Workflow.Lib.Specifications;

/// <summary>
/// Represents the data of workflow data.
/// </summary>
public class WorkflowData
{
    /// <summary>
    /// Description of the workflow
    /// </summary>
    /// <value></value>
    public string? Description { get; set; }

    /// <summary>
    /// Variables used in the workflow, if any
    /// </summary>
    public List<VariableData>? Variables { get; set; }

    /// <summary>
    /// Steps of the workflow
    /// </summary>
    public List<StepData> Steps { get; set; } = [];

    /// <summary>
    /// Subworkflows of the workflow, if any
    /// </summary>
    public Dictionary<string, WorkflowData>? Subworkflows { get; set; }
}
