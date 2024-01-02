using System;
using System.Threading.Tasks;

namespace KlabTestFramework.Workflow.Lib.Runner;

/// <summary>
/// Represents a workflow runner that executes a workflow.
/// </summary>
public interface IWorkflowRunner
{
    /// <summary>
    /// Event that is raised when the status of a workflow step changes.
    /// </summary>
    event EventHandler<WorkflowStepStatusEventArgs>? StepStatusChanged;

    /// <summary>
    /// Event that is raised when the workflow status changes.
    /// </summary>
    event EventHandler<WorkflowStatusEventArgs>? WorkflowStatusChanged;

    /// <summary>
    /// Runs the specified workflow asynchronously.
    /// </summary>
    /// <param name="workflow">The workflow to run.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<WorkflowResult> RunAsync(Specifications.Workflow workflow);
}

public record WorkflowResult(bool IsSuccess);

public class WorkflowStatusEventArgs : EventArgs
{
    public WorkflowStatus Status { get; set; }
}

public enum WorkflowStatus
{
    Idle,
    Running,
    Paused,
    Completed
}

/// <summary>
/// Provides data for the StepStatusChanged event.
/// </summary>
public class WorkflowStepStatusEventArgs : EventArgs
{
    /// <summary>
    /// Get or sets the id of the workflow step.
    /// </summary>
    public Guid StepId { get; set; }

    /// <summary>
    /// Gets or sets the status of the workflow step.
    /// </summary>
    public StepStatus Status { get; set; }
}

/// <summary>
/// Represents the status of a workflow step.
/// </summary>
public enum StepStatus
{
    ///
    /// Idle,
    Idle,

    /// <summary>
    /// The step is running.
    /// </summary>
    Running,

    /// <summary>
    /// The step is completed.
    /// </summary>
    Completed
}
