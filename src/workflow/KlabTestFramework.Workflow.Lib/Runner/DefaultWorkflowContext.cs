using System;
using System.Collections.Generic;
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
    public List<IVariable> Variables { get; set; } = new();

    /// <inheritdoc/>
    public void PublishMessage(string message)
    {
        Console.WriteLine(message);
    }
}
