using System;
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
    /// Author of the worklflow
    /// </summary>
    /// <value></value>
    public string Author { get; set; } = string.Empty;

    /// <summary>
    /// When the workflow was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.MinValue;

    /// <summary>
    /// When the workflow was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Variables used in the workflow, if any
    /// </summary>
    public List<VariableData>? Variables { get; set; }

    public Dictionary<string, WorkflowData>? Subworkflows { get; set; }

    /// <summary>
    /// Steps of the workflow
    /// </summary>
    public List<StepData> Steps { get; set; } = [];

}
