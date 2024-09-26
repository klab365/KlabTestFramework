using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Klab.Toolkit.Event;
using Klab.Toolkit.Results;
using KlabTestFramework.Workflow.Lib.Ports;
using KlabTestFramework.Workflow.Lib.Specifications;

namespace KlabTestFramework.Workflow.Lib.Features.Editor;

/// <summary>
/// Handler for querying a workflow.
/// </summary>
internal sealed class QueryWorkflowHandler : IRequestHandler<QueryWorkflowRequest, Specifications.Workflow>
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

    public async Task<Result<Specifications.Workflow>> HandleAsync(QueryWorkflowRequest request, CancellationToken cancellationToken)
    {
        if (!Path.Exists(request.FilePath))
        {
            return Result.Failure<Specifications.Workflow>(WorkflowModuleErrors.WorkflowNotFound(request.FilePath));
        }

        WorkflowData data = await _workflowRepository.GetWorkflowAsync(request.FilePath, cancellationToken);
        Specifications.Workflow workflow = CreateWorkflowFromData(new KeyValuePair<string, WorkflowData>(string.Empty, data));

        return Result.Success(workflow);
    }

    private Specifications.Workflow CreateWorkflowFromData(KeyValuePair<string, WorkflowData> data)
    {
        WorkflowData wfData = data.Value;

        Dictionary<string, Specifications.Workflow> subworkflows = LoadSubworkflows(wfData);
        IVariable[] variables = LoadVariables(wfData);
        IStep[] steps = LoadSteps(wfData, subworkflows);

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

    private IStep[] LoadSteps(WorkflowData wfData, Dictionary<string, Specifications.Workflow> subworkflows)
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

    private Dictionary<string, Specifications.Workflow> LoadSubworkflows(WorkflowData wfData)
    {
        return wfData.Subworkflows?
            .Select(s => new KeyValuePair<string, Specifications.Workflow>(s.Key, CreateWorkflowFromData(s)))
            .ToDictionary(k => k.Key, v => v.Value) ?? [];
    }
}

public record QueryWorkflowRequest(string FilePath) : IRequest;
