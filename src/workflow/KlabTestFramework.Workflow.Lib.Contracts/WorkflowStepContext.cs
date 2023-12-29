using System;
using System.Threading;

namespace KlabTestFramework.Workflow.Lib.Contracts;

public interface IWorkflowContext
{
    CancellationToken CancellationToken { get; }

    void PublishMessage(string message);
}

/// <summary>
/// Represents the context for a workflow step.
/// </summary>
public class WorkflowStepContext : IWorkflowContext
{
    /// <summary>
    /// Gets the cancellation token for the workflow step.
    /// </summary>
    public CancellationToken CancellationToken { get; }

    public void PublishMessage(string message)
    {
        Console.WriteLine(message);
    }
}
