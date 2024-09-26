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
internal sealed class QueryWorkflowHandler : IRequestHandler<QueryWorkflowRequest, IWorkflow>
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

    public async Task<Result<IWorkflow>> HandleAsync(QueryWorkflowRequest request, CancellationToken cancellationToken)
    {
        if (!Path.Exists(request.FilePath))
        {
            return Result.Failure<IWorkflow>(WorkflowModuleErrors.WorkflowNotFound(request.FilePath));
        }

        WorkflowData data = await _workflowRepository.GetWorkflowAsync(request.FilePath, cancellationToken);
        IWorkflow workflow = CreateWorkflowFromData(new KeyValuePair<string, WorkflowData>(string.Empty, data));

        return Result.Success(workflow);
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
}

public record QueryWorkflowRequest(string FilePath) : IRequest;
