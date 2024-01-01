using System.Collections.Generic;


namespace KlabTestFramework.Workflow.Lib.Specifications;

/// <summary>
/// Represents a workflow in the KlabTestFramework.
/// </summary>
public interface IWorkflow
{
    /// <summary>
    /// Gets or sets the metadata associated with the workflow.
    /// </summary>
    WorkflowData Metadata { get; }

    /// <summary>
    /// Gets the list of steps in the workflow.
    /// </summary>
    IReadOnlyList<StepContainer> Steps { get; }

    /// <summary>
    /// Gets the list of variables used in the workflow.
    /// </summary>
    IReadOnlyList<IVariable> Variables { get; }

    WorkflowData ToData();

    void FromData(WorkflowData data);
}
