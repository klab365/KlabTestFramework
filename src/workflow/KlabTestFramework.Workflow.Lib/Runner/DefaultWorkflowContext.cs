using System;
using System.Threading;

namespace KlabTestFramework.Workflow.Lib.Runner;

/// <summary>
/// Default implementation of <see cref="IWorkflowContext"/> interface.
/// </summary>
public class DefaultWorkflowContext : IWorkflowContext
{
    /// <inheritdoc/>
    public CancellationToken CancellationToken { get; }

    /// <inheritdoc/>
    public void PublishMessage(string message)
    {
        Console.WriteLine(message);
    }
}
