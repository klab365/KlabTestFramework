using System;
using System.Threading.Tasks;
using KlabTestFramework.Workflow.Lib.Specifications;

namespace KlabTestFramework.Workflow.Lib.Runner;

/// <summary>
/// Represents a workflow runner that executes a workflow.
/// </summary>
public interface IWorkflowRunner
{
    /// <summary>
    /// Event that is raised when the status of a workflow step changes.
    /// </summary>
    event Action<WorkflowStepStatusEvent>? StepStatusChanged;

    /// <summary>
    /// Event that is raised when the workflow status changes.
    /// </summary>
    event Action<WorkflowStatusEvent>? WorkflowStatusChanged;

    /// <summary>
    /// Runs the specified workflow asynchronously.
    /// </summary>
    /// <param name="workflow">The workflow to run.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<WorkflowResult> RunAsync(IWorkflow workflow, IWorkflowContext context);

    /// <summary>
    /// Runs the specified subworkflow asynchronously.
    /// </summary>
    /// <param name="subworkflowStep"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    Task<WorkflowResult> RunSubworkflowAsync(ISubworkflowStep subworkflowStep, IWorkflowContext context);
}

public record WorkflowResult(bool IsSuccess);

public record WorkflowStatusEvent(WorkflowStatus Status);

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
public record WorkflowStepStatusEvent(IStep Step, StepStatus Status);

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
