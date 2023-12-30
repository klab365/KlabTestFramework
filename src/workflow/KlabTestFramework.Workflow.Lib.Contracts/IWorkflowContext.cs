using System.Threading;

namespace KlabTestFramework.Workflow.Lib.Contracts;

/// <summary>
/// Represents the context for a workflow step.
/// </summary>
public interface IWorkflowContext
{
    /// <summary>
    /// Gets the cancellation token for the workflow step.
    /// </summary>
    CancellationToken CancellationToken { get; }

    void PublishMessage(string message);
}
