using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Klab.Toolkit.Results;
using KlabTestFramework.Workflow.Lib.Specifications;
using KlabTestFramework.Workflow.Lib.Validator;

namespace KlabTestFramework.Workflow.Lib.Editor;

/// <summary>
/// Implementation of <see cref="IWorkflowEditor"/> interface.
/// </summary>
public class WorkflowEditor : IWorkflowEditor
{
    private readonly List<IVariable> _variables = [];
    private readonly List<StepIndexContainer> _steps = [];
    private readonly Dictionary<string, IWorkflow> _subworkflows = [];
    private readonly IWorkflowRepository _repository;
    private readonly IWorkflowValidator _validator;
    private readonly IStepFactory _stepFactory;
    private readonly IParameterFactory _parameterFactory;
    private readonly IVariableFactory _variableFactory;
    private readonly List<Action<WorkflowData>> _workflowDataConfigurationCallbacks = [];

    public WorkflowEditor(
        IWorkflowRepository repository,
        IWorkflowValidator validator,
        IStepFactory stepFactory,
        IParameterFactory parameterFactory,
        IVariableFactory variableFactory)
    {
        _repository = repository;
        _validator = validator;
        _stepFactory = stepFactory;
        _parameterFactory = parameterFactory;
        _variableFactory = variableFactory;
    }

    /// <inheritdoc/>
    public void CreateNewWorkflow()
    {
        _workflowDataConfigurationCallbacks.Clear();
        _steps.Clear();
        _variables.Clear();
    }

    public void EditWorkflow(Specifications.Workflow workflow)
    {
        _steps.Clear();
        foreach (IStep step in workflow.Steps)
        {
            _steps.Add(new StepIndexContainer(step) { Index = _steps.Count });
        }

        _variables.Clear();
        foreach (IVariable variable in workflow.Variables)
        {
            _variables.Add(variable);
        }

        _workflowDataConfigurationCallbacks.Clear();
        _workflowDataConfigurationCallbacks.Add(wf => wf.Description = workflow.Metadata.Description);
        _workflowDataConfigurationCallbacks.Add(wf => wf.Author = workflow.Metadata.Author);
        _workflowDataConfigurationCallbacks.Add(wf => wf.CreatedAt = workflow.Metadata.CreatedAt);
        _workflowDataConfigurationCallbacks.Add(wf => wf.UpdatedAt = workflow.Metadata.UpdatedAt);
    }

    /// <inheritdoc/>
    public TStep AddStep<TStep>(Action<TStep>? configureCallback = default) where TStep : IStep
    {
        IStep step = _stepFactory.CreateStep<TStep>();
        ConfigureStepAfterCreate(step);

        configureCallback?.Invoke((TStep)step);
        _steps.Add(new StepIndexContainer(step) { Index = _steps.Count });
        return (TStep)step;
    }

    private void ConfigureStepAfterCreate(IStep step)
    {
        if (step is ISubworkflowStep subworkflowStep)
        {
            RegisterSelectionOfSubworklfow(subworkflowStep);
        }
    }

    private void RegisterSelectionOfSubworklfow(ISubworkflowStep subworkflowStep)
    {
        subworkflowStep.SubworkflowSelected += (sender, subworkflowName) =>
        {
            LoadSubworkflowByName(subworkflowStep, subworkflowName);
        };
    }

    private void LoadSubworkflowByName(ISubworkflowStep subworkflowStep, string subworkflowName)
    {
        if (_subworkflows.TryGetValue(subworkflowName, out IWorkflow? subworkflow))
        {
            subworkflowStep.Subworkflow = subworkflow;
        }
    }

    public void AddVariable<TParameter>(string name, string unit, VariableType variableType, Action<TParameter>? configureCallback = default!) where TParameter : IParameterType
    {
        TParameter parameterType = _parameterFactory.CreateParameterType<TParameter>();
        configureCallback?.Invoke(parameterType);

        VariableData data = new()
        {
            Name = name,
            Unit = unit,
            VariableType = variableType,
            DataType = parameterType.GetType().Name,
            Value = parameterType.AsString()
        };
        IVariable variable = _variableFactory.CreateNewVariableByType(data, parameterType);
        _variables.Add(variable);
    }

    /// <inheritdoc/>
    public void ConfigureMetadata(Action<WorkflowData> metaDataConfigureCallback)
    {
        _workflowDataConfigurationCallbacks.Add(metaDataConfigureCallback);
    }

    /// <inheritdoc/>
    public Task<Result<Specifications.Workflow>> BuildWorkflowAsync()
    {
        Specifications.Workflow workflow = CreateWorkflowByEditedData();
        ConfiugreTimestamps(workflow);

        Result<Specifications.Workflow> result = workflow;
        return Task.FromResult(result);
    }

    private Specifications.Workflow CreateWorkflowByEditedData()
    {
        IStep[] steps = _steps.Select(s => s.Step).ToArray();
        Specifications.Workflow workflow = new(steps, _variables.ToArray(), _subworkflows);

        foreach (Action<WorkflowData> callback in _workflowDataConfigurationCallbacks)
        {
            callback(workflow.Metadata);
        }

        return workflow;
    }

    private static void ConfiugreTimestamps(Specifications.Workflow workflow)
    {
        if (workflow.Metadata.CreatedAt == DateTime.MinValue)
        {
            workflow.Metadata.CreatedAt = DateTime.Now;
        }
        workflow.Metadata.UpdatedAt = DateTime.Now;
    }

    public async Task<Result> CheckWorkflowHasErrorsAsync(Specifications.Workflow workflow)
    {
        WorkflowValidatorResult result = await _validator.ValidateAsync(workflow);
        if (result.IsFailure)
        {
            return WorkflowEditorErrors.WorkflowIsNotValid;
        }

        return Result.Success();
    }

    /// <inheritdoc/>
    public async Task<Result<Specifications.Workflow>> LoadWorkflowFromFileAsync(string path)
    {
        WorkflowData data = await _repository.GetWorkflowAsync(path);
        Specifications.Workflow workflow = CreateWorkflowFromData(new KeyValuePair<string, WorkflowData>(string.Empty, data));
        return workflow;
    }

    private Specifications.Workflow CreateWorkflowFromData(KeyValuePair<string, WorkflowData> data)
    {
        WorkflowData wfData = data.Value;

        // steps
        IStep[] steps = wfData.Steps.Select(s =>
        {
            IStep step = _stepFactory.CreateStep(s);
            ConfigureStepAfterCreate(step);
            step.FromData(s);
            return step;
        })
        .ToArray();

        // variables
        IVariable[] variables = wfData.Variables?.Select(v => _variableFactory.CreateVariableFromData(v)).ToArray() ?? [];

        // subworkflows (recursion!)
        Dictionary<string, IWorkflow> subworkflows = wfData.Subworkflows?
            .Select(s => new KeyValuePair<string, IWorkflow>(s.Key, CreateWorkflowFromData(s)))
            .ToDictionary(k => k.Key, v => v.Value) ?? new();

        Specifications.Workflow workflow = new(steps, variables, subworkflows) { Metadata = wfData };
        return workflow;
    }

    /// <inheritdoc/>
    public async Task<Result> SaveWorkflowAsync(string path, Specifications.Workflow workflow)
    {
        WorkflowData data = GetWorkflowDataFromWorkflow(workflow);
        await _repository.SaveWorkflowAsync(path, data);
        return Result.Success();
    }

    public void IncludeSubworkflow(string name, IWorkflow workflow)
    {
        _subworkflows.Add(name, workflow);
    }

    private static WorkflowData GetWorkflowDataFromWorkflow(Specifications.Workflow workflow)
    {
        return workflow.ToData();
    }
}

/// <summary>
/// Represents a container for a step in a workflow editor.
/// This class is used to know the order of the steps in the workflow.
/// </summary>
public class StepIndexContainer(IStep step)
{
    /// <summary>
    /// Gets or sets the order index.
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// Gets the step of the workflow.
    /// </summary>
    public IStep Step { get; } = step;
}
