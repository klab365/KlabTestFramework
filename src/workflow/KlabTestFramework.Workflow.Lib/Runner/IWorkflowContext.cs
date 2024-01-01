using System.Threading;

namespace KlabTestFramework.Workflow.Lib.Runner;

/// <summary>
/// Represents the context for a workflow step.
/// </summary>
public interface IWorkflowContext
{
    /// <summary>
    /// Gets the cancellation token for the workflow step.
    /// </summary>
    CancellationToken CancellationToken { get; }

    /// <summary>
    /// Publishes a message.
    /// </summary>
    /// <param name="message">The message to be published.</param>
    void PublishMessage(string message);
}
