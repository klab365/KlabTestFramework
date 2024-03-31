using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Klab.Toolkit.Results;
using KlabTestFramework.Shared.Parameters;
using KlabTestFramework.Workflow.Lib.Specifications;

namespace KlabTestFramework.Workflow.Lib.Editor;

/// <summary>
/// Implementation of <see cref="IWorkflowEditor"/> interface.
/// </summary>
public class WorkflowEditor : IWorkflowEditor
{
    private readonly List<IVariable> _variables = [];
    private readonly List<IStep> _steps = [];
    private readonly Dictionary<string, IWorkflow> _subworkflows = [];
    private readonly IWorkflowRepository _repository;
    private readonly StepFactory _stepFactory;
    private readonly ParameterFactory _parameterFactory;
    private readonly VariableFactory _variableFactory;
    private readonly List<Action<WorkflowData>> _workflowDataConfigurationCallbacks = [];

    public WorkflowEditor(
        IWorkflowRepository repository,
        StepFactory stepFactory,
        ParameterFactory parameterFactory,
        VariableFactory variableFactory)
    {
        _repository = repository;
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
        _subworkflows.Clear();
    }

    public void EditWorkflow(IWorkflow workflow)
    {
        CreateNewWorkflow();

        foreach (IStep step in workflow.Steps)
        {
            _steps.Add(step);
        }

        foreach (IVariable variable in workflow.Variables)
        {
            _variables.Add(variable);
        }

        foreach (KeyValuePair<string, IWorkflow> subworkflow in workflow.Subworkflows)
        {
            _subworkflows.Add(subworkflow.Key, subworkflow.Value);
        }

        _workflowDataConfigurationCallbacks.Add(wf => wf.Description = workflow.Metadata.Description);
    }

    /// <inheritdoc/>
    public TStep AddStepToLastPosition<TStep>(Action<TStep>? configureCallback = default) where TStep : IStep
    {
        IStep step = _stepFactory.CreateStep<TStep>();
        ConfigureStepAfterCreate(step);

        configureCallback?.Invoke((TStep)step);
        _steps.Add(step);
        return (TStep)step;
    }

    public Result AddChildStepToLastPosition(IStepWithChildren parentStep, string stepKey, Action<IStep>? configureCallback = default!)
    {
        StepData stepData = new() { Type = stepKey };
        IStep childStep = _stepFactory.CreateStep(stepData);
        ConfigureStepAfterCreate(childStep);
        configureCallback?.Invoke(childStep);

        parentStep.Children.Add(childStep);
        return Result.Success();
    }

    public Result MoveStepUp(IStep step)
    {
        int foundIndex = _steps.FindIndex(s => s.Id == step.Id);
        if (foundIndex == -1)
        {
            return WorkflowEditorErrors.StepNotFound;
        }

        bool isStepAtEndPosition = foundIndex == _steps.Count - 1;
        if (isStepAtEndPosition)
        {
            return WorkflowEditorErrors.StepIsAtEndPosition;
        }

        IStep previousStep = _steps[foundIndex - 1];
        _steps[foundIndex - 1] = step;
        _steps[foundIndex] = previousStep;
        return Result.Success();
    }

    public Result MoveChildStepUp(IStepWithChildren parentStep, IStep childStep)
    {
        int foundParentIndex = _steps.FindIndex(s => s.Id == parentStep.Id);
        if (foundParentIndex == -1)
        {
            return WorkflowEditorErrors.StepNotFound;
        }

        int foundChildIndex = parentStep.Children.FindIndex(s => s.Id == childStep.Id);
        if (foundChildIndex == -1)
        {
            return WorkflowEditorErrors.StepNotFound;
        }

        bool isStepAtEndPosition = foundChildIndex == parentStep.Children.Count - 1;
        if (isStepAtEndPosition)
        {
            return WorkflowEditorErrors.StepIsAtEndPosition;
        }

        IStep previousStep = parentStep.Children[foundChildIndex - 1];
        parentStep.Children[foundChildIndex - 1] = childStep;
        parentStep.Children[foundChildIndex] = previousStep;
        return Result.Success();
    }

    public Result MoveStepDown(IStep step)
    {
        int foundIndex = _steps.FindIndex(s => s.Id == step.Id);
        if (foundIndex == -1)
        {
            return WorkflowEditorErrors.StepNotFound;
        }

        bool isStepAtFirstPosition = foundIndex == 0;
        if (isStepAtFirstPosition)
        {
            return WorkflowEditorErrors.StepIsAtFirstPosition;
        }

        IStep nextStep = _steps[foundIndex + 1];
        _steps[foundIndex + 1] = step;
        _steps[foundIndex] = nextStep;
        return Result.Success();
    }

    public Result MoveChildStepDown(IStepWithChildren parentStep, IStep childStep)
    {
        int foundIndex = _steps.FindIndex(s => s.Id == parentStep.Id);
        if (foundIndex == -1)
        {
            return WorkflowEditorErrors.StepNotFound;
        }

        int foundChildIndex = parentStep.Children.FindIndex(s => s.Id == childStep.Id);
        if (foundChildIndex == -1)
        {
            return WorkflowEditorErrors.StepNotFound;
        }

        bool isStepAtFirstPosition = foundChildIndex == 0;
        if (isStepAtFirstPosition)
        {
            return WorkflowEditorErrors.StepIsAtFirstPosition;
        }

        IStep nextStep = parentStep.Children[foundChildIndex + 1];
        parentStep.Children[foundChildIndex + 1] = childStep;
        parentStep.Children[foundChildIndex] = nextStep;
        return Result.Success();
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
        IVariable variable = VariableFactory.CreateNewVariableByType(data, parameterType);
        _variables.Add(variable);
    }

    /// <inheritdoc/>
    public void ConfigureMetadata(Action<WorkflowData> metaDataConfigureCallback)
    {
        _workflowDataConfigurationCallbacks.Add(metaDataConfigureCallback);
    }

    /// <inheritdoc/>
    public Task<Result<IWorkflow>> BuildWorkflowAsync()
    {
        AssignIdsToSteps(_steps);
        IWorkflow workflow = CreateWorkflowByEditedData();
        Specifications.Workflow workflow1 = (Specifications.Workflow)workflow;
        Result<IWorkflow> result = workflow1;
        return Task.FromResult(result);
    }

    /// <inheritdoc/>
    public async Task<Result> LoadWorkflowFromFileAsync(string path)
    {
        WorkflowData data = await _repository.GetWorkflowAsync(path);
        IWorkflow workflow = CreateWorkflowFromData(new KeyValuePair<string, WorkflowData>(string.Empty, data));
        EditWorkflow(workflow);

        return Result.Success();
    }

    /// <inheritdoc/>
    public async Task<Result> SaveWorkflowAsync(string path)
    {
        Result<IWorkflow> buildWorkflowResult = await BuildWorkflowAsync();
        if (buildWorkflowResult.IsFailure)
        {
            return WorkflowEditorErrors.WorkflowIsNotValid;
        }

        IWorkflow workflow = buildWorkflowResult.Value!;
        WorkflowData data = GetWorkflowDataFromWorkflow(workflow);
        await _repository.SaveWorkflowAsync(path, data);
        return Result.Success();
    }

    /// <inheritdoc/>
    public void IncludeSubworkflow(string name, IWorkflow workflow)
    {
        _subworkflows.Add(name, workflow);
    }

    private IWorkflow CreateWorkflowByEditedData()
    {
        IWorkflow workflow = new Specifications.Workflow(_steps.ToArray(), _variables.ToArray(), _subworkflows);

        foreach (Action<WorkflowData> callback in _workflowDataConfigurationCallbacks)
        {
            callback(workflow.Metadata);
        }

        return workflow;
    }

    /// <summary>
    /// Assigns unique ids to the steps of the workflow, if they don't have one.
    /// </summary>
    /// <param name="steps"></param>
    private static void AssignIdsToSteps(IEnumerable<IStep> steps, ISubworkflowStep? subworkflowStepInput = default)
    {
        foreach (IStep step in steps)
        {
            if (step.Id.IsEmpty)
            {
                string id = $"{step.GetType().Name}_{Guid.NewGuid()}";
                step.Id = StepId.Create(id);
            }
            else if (step.Id.IsCustom && subworkflowStepInput != null)
            {
                step.Id.AddRoute(subworkflowStepInput.Id.Value);
            }

            if (step is ISubworkflowStep subworkflowStep)
            {
                AssignIdsToSteps(subworkflowStep.Children, subworkflowStep);
            }
        }
    }

    private IWorkflow CreateWorkflowFromData(KeyValuePair<string, WorkflowData> data)
    {
        WorkflowData wfData = data.Value;

        Dictionary<string, IWorkflow> subworkflows = LoadSubworkflows(wfData);
        IVariable[] variables = LoadVariables(wfData);
        IStep[] steps = LoadSteps(wfData, subworkflows);

        IWorkflow workflow = new Specifications.Workflow(steps, variables, subworkflows) { Metadata = wfData };
        return workflow;
    }

    private IVariable[] LoadVariables(WorkflowData wfData)
    {
        return wfData.Variables?
            .Select(v => _variableFactory.CreateVariableFromData(v))
            .ToArray() ?? [];
    }

    private IStep[] LoadSteps(WorkflowData wfData, Dictionary<string, IWorkflow> subworkflows)
    {
        List<IStep> steps = new();
        foreach (StepData stepData in wfData.Steps)
        {
            IStep step = _stepFactory.CreateStep(stepData);
            if (step is ISubworkflowStep subworkflowStep)
            {
                ParameterData parameterData = stepData.Parameters!.FoundParameterDataByName(subworkflowStep.SelectedSubworkflow.Name);
                string subworkflowName = parameterData.Value;
                subworkflowStep.SelectedSubworkflow.Content.AddOptions(subworkflows.Keys.ToArray());
                subworkflowStep.Subworkflow = subworkflows[subworkflowName];
            }

            step.FromData(stepData);
            steps.Add(step);
        }

        return steps.ToArray();
    }

    private Dictionary<string, IWorkflow> LoadSubworkflows(WorkflowData wfData)
    {
        return wfData.Subworkflows?
            .Select(s => new KeyValuePair<string, IWorkflow>(s.Key, CreateWorkflowFromData(s)))
            .ToDictionary(k => k.Key, v => v.Value) ?? [];
    }

    private static WorkflowData GetWorkflowDataFromWorkflow(IWorkflow workflow)
    {
        return workflow.ToData();
    }

    private void ConfigureStepAfterCreate(IStep step)
    {
        if (step is ISubworkflowStep subworkflowStep)
        {
            subworkflowStep.SelectedSubworkflow.Content.AddOptions(_subworkflows.Keys.ToArray());
            subworkflowStep.SubworkflowSelected += (subworkflowName) => LoadSubworkflowByName(subworkflowStep, subworkflowName);
        }
    }

    private void LoadSubworkflowByName(ISubworkflowStep subworkflowStep, string subworkflowName)
    {
        if (_subworkflows.TryGetValue(subworkflowName, out IWorkflow? subworkflow))
        {
            subworkflowStep.Subworkflow = subworkflow;
        }
    }
}
