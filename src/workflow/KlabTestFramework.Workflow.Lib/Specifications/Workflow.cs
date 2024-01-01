using System;
using System.Collections.Generic;


namespace KlabTestFramework.Workflow.Lib.Specifications;

/// <summary>
/// Represents a workflow that consists of multiple steps.
/// </summary>
public class Workflow
{
    private readonly List<StepContainer> _steps = new();

    private readonly List<IVariable> _variables = new();

    public WorkflowData Metadata { get; set; } = new();

    /// <summary>
    /// Gets the read-only list of steps in the workflow.
    /// </summary>
    public IReadOnlyList<StepContainer> Steps => _steps.AsReadOnly();

    public IReadOnlyList<IVariable> Variables => _variables.AsReadOnly();

    public Workflow(IStep[] steps, IVariable[] variables)
    {
        AddSteps(steps);
        AddVariables(variables);
    }

    private void AddVariables(IVariable[] variables)
    {
        _variables.AddRange(variables);
    }

    private void AddSteps(IStep[] steps)
    {
        foreach (IStep step in steps)
        {
            _steps.Add(new StepContainer() { Step = step });
        }
    }
}

/// <summary>
/// Represents a container for a step in a workflow.
/// </summary>
public class StepContainer
{
    /// <summary>
    /// Gets or sets the unique identifier of the step container.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the step associated with the container.
    /// </summary>
    public required IStep Step { get; set; }
}
