using System.Collections.Generic;

namespace KlabTestFramework.Workflow.Abstractions.Specifications;

/// <summary>
/// Default implementation of <see cref="IWorkflowContext"/> interface.
/// </summary>
public class WorkflowContext
{
    private readonly List<IVariable> _runTimeVariables = new();
    public IEnumerable<IVariable> RunTimeVariables => _runTimeVariables;
}
