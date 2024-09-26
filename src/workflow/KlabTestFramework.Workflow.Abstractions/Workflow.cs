using System.Collections.Generic;

namespace KlabTestFramework.Workflow.Lib.Specifications;

/// <summary>
/// Represents a workflow that consists of multiple steps.
/// </summary>
public class Workflow
{
    /// <summary>
    /// Gets the read-only list of steps in the workflow.
    /// </summary>
    public List<IStep> Steps { get; } = new();

    /// <summary>
    /// Gets the read-only list of variables used in the workflow.
    /// </summary>
    public List<IVariable> Variables { get; } = new();

    /// <summary>
    /// Gets the read-only list of subworkflows used in the workflow as a dictionary.
    /// </summary>
    public Dictionary<string, Workflow> Subworkflows { get; } = new();
}
