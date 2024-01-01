using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Klab.Toolkit.Results;

using KlabTestFramework.Workflow.Lib.Specifications;

namespace KlabTestFramework.Workflow.Lib.Editor;

/// <summary>
/// Implementation of <see cref="IWorkflowEditor"/>.
/// </summary>
public class WorkflowEditor : IWorkflowEditor
{
    private readonly List<IVariable> _variables = new();
    private readonly List<StepIndexContainer> _steps = new();
    private readonly IWorkflowRepository _repository;
    private readonly IStepFactory _stepFactory;
    private readonly IParameterFactory _parameterFactory;
    private readonly List<Action<WorkflowData>> _metaDataConfigureCallbacks = new();

    public WorkflowEditor(IWorkflowRepository repository, IStepFactory stepFactory, IParameterFactory parameterFactory)
    {
        _repository = repository;
        _stepFactory = stepFactory;
        _parameterFactory = parameterFactory;
    }

    public void CreateNewWorkflow()
    {
        _metaDataConfigureCallbacks.Clear();
        _steps.Clear();
        _variables.Clear();
    }

    /// <inheritdoc/>
    public void AddStep<TStep>(Action<TStep>? configureCallback = default) where TStep : IStep
    {
        IStep step = _stepFactory.CreateStep<TStep>();
        configureCallback?.Invoke((TStep)step);
        _steps.Add(new StepIndexContainer(step) { Index = _steps.Count });
    }

    public void AddVariable<TParameter>(string name, Action<TParameter>? configureCallback = default!) where TParameter : IParameterType
    {
        TParameter variable = _parameterFactory.CreateParameterType<TParameter>();
        configureCallback?.Invoke(variable);
        Variable<TParameter> defaultVariable = new(name, variable);
        _variables.Add(defaultVariable);
    }

    /// <inheritdoc/>
    public void ConfigureMetadata(Action<WorkflowData> metaDataConfigureCallback)
    {
        _metaDataConfigureCallbacks.Add(metaDataConfigureCallback);
    }

    /// <inheritdoc/>
    public Result<Specifications.Workflow> BuildWorkflow()
    {
        IStep[] steps = _steps.Select(s => s.Step).ToArray();
        Specifications.Workflow workflow = new(steps, _variables.ToArray());
        foreach (Action<WorkflowData> callback in _metaDataConfigureCallbacks)
        {
            callback(workflow.Metadata);
        }
        return workflow;
    }

    /// <inheritdoc/>
    public async Task<Result<Specifications.Workflow>> LoadWorkflowFromFileAsync(string path)
    {
        WorkflowData data = await _repository.GetWorkflowAsync(path);
        IStep[] steps = data.Steps.Select(s => _stepFactory.CreateStep(s)).ToArray();
        Specifications.Workflow workflow = new(steps, Array.Empty<IVariable>());

        return workflow;
    }

    /// <inheritdoc/>
    public async Task<Result> SaveWorkflowAsync(string path, Specifications.Workflow workflow)
    {
        WorkflowData data = GetWorkflowDataFromWorkflow(workflow);
        await _repository.SaveWorkflowAsync(path, data);
        return Result.Success();
    }

    private WorkflowData GetWorkflowDataFromWorkflow(Specifications.Workflow workflow)
    {
        WorkflowData data = workflow.Metadata;
        data.Variables = _variables.Select(v => v.ToData()).ToList();
        data.Steps = GetStepData();
        return data;
    }

    private List<StepData> GetStepData()
    {
        List<StepData> stepData = new();
        foreach (IStep step in _steps.Select(s => s.Step))
        {
            StepData singleStepData = new() { Type = step.GetType().Name, };
            IEnumerable<ParameterContainer>? parameters = step.GetParameters();
            if (parameters is not null)
            {
                List<ParameterData> parameterData = new();
                foreach (ParameterContainer parameter in parameters)
                {
                    ParameterData newParameterData = new()
                    {
                        Name = parameter.Key,
                        IsVariable = parameter.Value.IsVariable ? true : null,
                        Value = parameter.Value.ToData()
                    };

                    parameterData.Add(newParameterData);
                }
                singleStepData.Parameters = parameterData;
            }

            stepData.Add(singleStepData);
        }

        return stepData;
    }
}

/// <summary>
/// Represents a container for a step in a workflow.
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
