using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Klab.Toolkit.Results;
using KlabTestFramework.Workflow.Lib.Contracts;
using KlabTestFramework.Workflow.Lib.Specifications;

namespace KlabTestFramework.Workflow.Lib.Editor;

/// <summary>
/// Implementation of <see cref="IWorkflowEditor"/>.
/// </summary>
public class WorkflowEditor : IWorkflowEditor
{
    private readonly List<StepIndexContainer> _steps = new();
    private readonly IWorkflowRepository _repository;
    private readonly IStepFactory _stepFactory;
    private readonly List<Action<WorkflowData>> _metaDataConfigureCallbacks = new();
    private bool _canWorkflowBuilt;

    public WorkflowEditor(IWorkflowRepository repository, IStepFactory stepFactory)
    {
        _repository = repository;
        _stepFactory = stepFactory;
    }

    public void CreateNewWorkflow()
    {
        _canWorkflowBuilt = true;
        _metaDataConfigureCallbacks.Clear();
        _steps.Clear();
    }

    /// <inheritdoc/>
    public void AddStep<TStep>(Action<TStep>? configureCallback = default) where TStep : IStep
    {
        IStep step = _stepFactory.CreateStep<TStep>();
        configureCallback?.Invoke((TStep)step);
        _steps.Add(new StepIndexContainer(step) { Index = _steps.Count });
    }

    /// <inheritdoc/>
    public void ConfigureMetadata(Action<WorkflowData> metaDataConfigureCallback)
    {
        _metaDataConfigureCallbacks.Add(metaDataConfigureCallback);
    }

    /// <inheritdoc/>
    public Result<Specifications.Workflow> BuildWorkflow()
    {
        if (!_canWorkflowBuilt)
        {
            return new Error(1, "Workflow cannot be built", $"Have you called {nameof(CreateNewWorkflow)}?");
        }

        Specifications.Workflow workflow = new(_steps.Select(s => s.Step).ToArray());
        foreach (Action<WorkflowData> callback in _metaDataConfigureCallbacks)
        {
            callback(workflow.Metadata);
        }
        _canWorkflowBuilt = false;
        return workflow;
    }

    /// <inheritdoc/>
    public async Task<Result<Specifications.Workflow>> LoadWorkflowFromFileAsync(string path)
    {
        WorkflowData data = await _repository.GetWorkflowAsync(path);
        IStep[] steps = data.Steps.Select(s => _stepFactory.CreateStep(s)).ToArray();
        Specifications.Workflow workflow = new(steps);
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
        data.Steps = GetStepData();
        return data;
    }

    private List<StepData> GetStepData()
    {
        List<StepData> stepData = new();
        foreach (IStep step in _steps.Select(s => s.Step))
        {
            StepData singleStepData = new() { Type = step.GetType().Name, };
            IEnumerable<ParameterData>? parameters = step.GetParameterData();
            if (parameters is not null)
            {
                singleStepData.Parameters = parameters.ToList();
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
