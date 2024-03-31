using System.Collections.Generic;
using System.Linq;


namespace KlabTestFramework.Workflow.Lib.Specifications;

/// <summary>
/// Represents a workflow that consists of multiple steps.
/// </summary>
internal class Workflow : IWorkflow
{
    private readonly List<IStep> _steps = new();

    private readonly List<IVariable> _variables = new();

    private readonly Dictionary<string, IWorkflow> _subworkflows = new();

    public WorkflowData Metadata { get; set; } = new();

    /// <summary>
    /// Gets the read-only list of steps in the workflow.
    /// </summary>
    public IReadOnlyList<IStep> Steps => _steps.AsReadOnly();

    public IReadOnlyList<IVariable> Variables => _variables.AsReadOnly();

    public IReadOnlyDictionary<string, IWorkflow> Subworkflows => _subworkflows.AsReadOnly();

    public Workflow(IStep[] steps, IVariable[] variables, IReadOnlyDictionary<string, IWorkflow> subworkflows)
    {
        AddSteps(steps);
        AddVariables(variables);
        AddSubworkflows(subworkflows);
    }

    private void AddSubworkflows(IReadOnlyDictionary<string, IWorkflow> subworkflows)
    {
        foreach (KeyValuePair<string, IWorkflow> subworkflow in subworkflows)
        {
            _subworkflows.Add(subworkflow.Key, subworkflow.Value);
        }
    }

    private void AddVariables(IVariable[] variables)
    {
        _variables.AddRange(variables);
    }

    private void AddSteps(IStep[] steps)
    {
        _steps.AddRange(steps);
    }

    public WorkflowData ToData()
    {
        WorkflowData data = Metadata;

        data.Steps = Steps.Select(s => s.ToData()).ToList();

        if (Variables.Any())
        {
            data.Variables = Variables.Select(v => v.ToData()).ToList();
        }

        if (Subworkflows.Any())
        {
            data.Subworkflows = Subworkflows
                .Select(s => new KeyValuePair<string, WorkflowData>(s.Key, s.Value.ToData()))
                .ToDictionary(k => k.Key, v => v.Value);
        }

        return data;
    }
}
