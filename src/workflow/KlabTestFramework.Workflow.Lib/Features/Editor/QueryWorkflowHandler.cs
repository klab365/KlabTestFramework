using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Klab.Toolkit.Event;
using Klab.Toolkit.Results;
using KlabTestFramework.Workflow.Lib.BuiltIn;
using KlabTestFramework.Workflow.Lib.Ports;
using KlabTestFramework.Workflow.Lib.Specifications;

namespace KlabTestFramework.Workflow.Lib.Features.Editor;

/// <summary>
/// Handler for querying a workflow.
/// </summary>
internal sealed class QueryWorkflowHandler :
    IRequestHandler<QueryWorkflowRequest, Specifications.Workflow>,
    IRequestHandler<QueryWorkflowRequestByData, Specifications.Workflow>
{
    private readonly IWorkflowRepository _workflowRepository;
    private readonly StepFactory _stepFactory;
    private readonly VariableFactory _variableFactory;

    public QueryWorkflowHandler(IWorkflowRepository workflowRepository, StepFactory stepFactory, VariableFactory variableFactory)
    {
        _workflowRepository = workflowRepository;
        _stepFactory = stepFactory;
        _variableFactory = variableFactory;
    }

    /// <summary>
    /// Handle the request to query a workflow with a file path as input
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Result<Specifications.Workflow>> HandleAsync(QueryWorkflowRequest request, CancellationToken cancellationToken)
    {
        if (!Path.Exists(request.FilePath))
        {
            return Result.Failure<Specifications.Workflow>(WorkflowModuleErrors.WorkflowNotFound(request.FilePath));
        }

        try
        {
            WorkflowData data = await _workflowRepository.GetWorkflowAsync(request.FilePath, cancellationToken);
            Specifications.Workflow workflow = await CreateWorkflowFromDataAsync(data, cancellationToken);
            return Result.Success(workflow);
        }
        catch (Exception ex)
        {
            return Result.Failure<Specifications.Workflow>(WorkflowModuleErrors.WorkflowLoadError(request.FilePath, ex));
        }
    }

    /// <summary>
    /// Handle the request to query a workflow with data as input
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Result<Specifications.Workflow>> HandleAsync(QueryWorkflowRequestByData request, CancellationToken cancellationToken)
    {
        // clone the data to avoid modifying the original data
        WorkflowData copy = await _workflowRepository.CopyAsync(request.Data, cancellationToken);
        Specifications.Workflow workflow = await CreateWorkflowFromDataAsync(copy, cancellationToken);
        return Result.Success(workflow);
    }

    private async Task<Specifications.Workflow> CreateWorkflowFromDataAsync(WorkflowData data, CancellationToken cancellationToken)
    {
        Dictionary<string, Specifications.Workflow> subworkflows = await LoadSubworkflowsAsync(data, cancellationToken);
        IVariable[] variables = LoadVariables(data);
        IStep[] steps = await LoadStepsAsync(data, cancellationToken);

        Specifications.Workflow workflow = new();
        workflow.Variables.AddRange(variables);
        workflow.Steps.AddRange(steps);
        foreach (KeyValuePair<string, Specifications.Workflow> subworkflow in subworkflows)
        {
            workflow.Subworkflows.Add(subworkflow.Key, subworkflow.Value);
        }

        return workflow;
    }

    private IVariable[] LoadVariables(WorkflowData wfData)
    {
        return wfData.Variables?
            .Select(v => _variableFactory.CreateVariableFromData(v))
            .ToArray() ?? [];
    }

    private async Task<IStep[]> LoadStepsAsync(WorkflowData wfData, CancellationToken cancellationToken)
    {
        List<IStep> steps = new();
        foreach (StepData stepData in wfData.Steps)
        {
            IStep step = _stepFactory.CreateStep(stepData);

            // handle subworkflow step
            if (step is SubworkflowStep subworkflowStep)
            {
                subworkflowStep.WorkflowData = wfData.Subworkflows ?? new();
                string subworkflowName = stepData.Parameters?.Find(p => p.Name == "Subworkflow")?.Value ?? string.Empty;
                subworkflowStep.SelectedSubworkflow.Content.AddOptions(wfData.Subworkflows?.Keys.ToArray() ?? []);
                WorkflowData? subworkflowData = wfData.Subworkflows?.GetValueOrDefault(subworkflowName);
                if (subworkflowData is null)
                {
                    continue;
                }

                subworkflowStep.AddSubworkflowOptions(wfData.Subworkflows?.Keys.ToArray() ?? []);
                await subworkflowStep.UpdateSubworkflowAsync(subworkflowName, cancellationToken);
            }

            AssignDataToStep(step, stepData);

            steps.Add(step);
        }

        return steps.ToArray();
    }

    private static void AssignDataToStep(IStep step, StepData stepData)
    {
        step.FromData(stepData);
    }

    private async Task<Dictionary<string, Specifications.Workflow>> LoadSubworkflowsAsync(WorkflowData wfData, CancellationToken cancellationToken)
    {
        Dictionary<string, Specifications.Workflow> subworkflows = new();

        foreach (KeyValuePair<string, WorkflowData> subworkflow in wfData.Subworkflows ?? new())
        {
            Specifications.Workflow workflow = await CreateWorkflowFromDataAsync(subworkflow.Value, cancellationToken);
            subworkflows.Add(subworkflow.Key, workflow);
        }

        return subworkflows;
    }
}

public record QueryWorkflowRequest(string FilePath) : IRequest<Specifications.Workflow>;

public record QueryWorkflowRequestByData(WorkflowData Data) : IRequest<Specifications.Workflow>;
