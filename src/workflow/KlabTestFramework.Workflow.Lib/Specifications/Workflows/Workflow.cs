using System.Collections.Generic;
using System.Linq;


namespace KlabTestFramework.Workflow.Lib.Specifications;

/// <summary>
/// Represents a workflow that consists of multiple steps.
/// </summary>
public class Workflow : IWorkflow
{
    private readonly List<StepContainer> _steps = new();

    private readonly List<IVariable> _variables = new();

    public WorkflowData Metadata { get; set; } = new();

    /// <summary>
    /// Gets the read-only list of steps in the workflow.
    /// </summary>
    public IReadOnlyList<StepContainer> Steps => _steps.AsReadOnly();

    public IReadOnlyList<IVariable> Variables => _variables.AsReadOnly();

    public Workflow(StepContainer[] steps, IVariable[] variables)
    {
        AddSteps(steps);
        AddVariables(variables);
    }

    private void AddVariables(IVariable[] variables)
    {
        _variables.AddRange(variables);
    }

    private void AddSteps(StepContainer[] steps)
    {
        _steps.AddRange(steps);
    }

    public WorkflowData ToData()
    {
        WorkflowData data = Metadata;
        data.Steps = Steps.Select(s => s.ToData()).ToList();
        data.Variables = Variables.Select(v => v.ToData()).ToList();
        return data;
    }

    public void FromData(WorkflowData data)
    {
        throw new System.NotImplementedException();
    }
}
