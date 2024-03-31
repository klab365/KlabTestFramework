using System;
using System.Threading;
using KlabTestFramework.Workflow.Lib.Specifications;

namespace KlabTestFramework.Workflow.Lib.Runner;

/// <summary>
/// Default implementation of <see cref="IWorkflowContext"/> interface.
/// </summary>
public class DefaultWorkflowContext : IWorkflowContext
{
    /// <inheritdoc/>
    public CancellationToken CancellationToken { get; }

    /// <inheritdoc/>
    public IVariable[] Variables { get; set; } = Array.Empty<IVariable>();

    /// <inheritdoc/>
    public void PublishMessage(IStep step, string message)
    {
        string id = step.Id.Value;
        string composedMessage = $"[{id}] {message}";
        Console.WriteLine(composedMessage);
    }
}
