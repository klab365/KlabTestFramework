using System;
using System.Threading;

namespace KlabTestFramework.Workflow.Lib.Contracts;

/// <summary>
/// Implementation of <see cref="IWorkflowContext"/>.
/// </summary>
public class WorkflowStepContext : IWorkflowContext
{
    /// <inheritdoc/>
    public CancellationToken CancellationToken { get; }

    /// <inheritdoc/>
    public void PublishMessage(string message)
    {
        Console.WriteLine(message);
    }
}
