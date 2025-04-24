using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Klab.Toolkit.Event;
using Klab.Toolkit.Results;
using KlabTestFramework.Shared.Parameters;
using KlabTestFramework.Shared.Parameters.Types;
using KlabTestFramework.Workflow.Abstractions;
using KlabTestFramework.Workflow.Abstractions.Specifications;
using KlabTestFramework.Workflow.Lib.Features.Editor;
using KlabTestFramework.Workflow.Lib.Specifications;


namespace KlabTestFramework.Workflow.Lib.BuiltIn;

internal class SubworkflowStep : ISubworkflowStep
{
    public static readonly StringParameter NoneSelected = new() { Name = "none" };
    private readonly IEventBus _eventBus;

    public StepId Id { get; set; } = StepId.Empty;

    public StepParameter<SelectableParameter<StringParameter>> SelectedSubworkflow { get; }

    private readonly List<IStepParameter> _arguments = new();
    public IEnumerable<IStepParameter> Arguments => _arguments;

    public Abstractions.Specifications.Workflow Subworkflow { get; private set; } = new();

    public IEnumerable<IStep> Steps => Subworkflow.Steps;

    public Dictionary<string, WorkflowData> WorkflowData { get; internal set; } = new();

    public SubworkflowStep(ParameterFactory parameterFactory, IEventBus eventBus)
    {
        SelectedSubworkflow = parameterFactory.CreateParameter<SelectableParameter<StringParameter>>
        (
            "Subworkflow",
            string.Empty,
            p => p.SetValue(NoneSelected)
        );
        _eventBus = eventBus;
    }

    public IEnumerable<IStepParameter> GetParameters()
    {
        yield return SelectedSubworkflow;

        foreach (IStepParameter args in Arguments)
        {
            yield return args;
        }
    }

    public void SelectSubworkflow(string subWorkflow)
    {
        StringParameter parameter = new();
        parameter.SetValue(subWorkflow);
        SelectedSubworkflow.Content.SelectOption(parameter);
    }

    public async Task<Result> UpdateSubworkflowAsync(string wfName, CancellationToken cancellationToken = default)
    {
        WorkflowData? wfData = WorkflowData.GetValueOrDefault(wfName);
        if (wfData is null)
        {
            return Result.Failure(WorkflowModuleErrors.SubworkflowNotFound(wfName));
        }

        QueryWorkflowRequestByData req = new(wfData);
        Result<Abstractions.Specifications.Workflow> res = await _eventBus.SendAsync(req, cancellationToken);
        if (res.IsFailure)
        {
            return res;
        }

        Subworkflow = res.Value;
        UpdateArguments();
        return Result.Success();
    }

    public void AddSubworkflowOptions(params string[] subworkflows)
    {
        SelectedSubworkflow.Content.AddOptions(subworkflows);
    }

    public void RemoveSubworkflowOptions(params string[] subworkflows)
    {
        SelectedSubworkflow.Content.RemoveOptions(subworkflows);
    }

    private void UpdateArguments()
    {
        _arguments.Clear();
        foreach (IVariable variable in Subworkflow.Variables.Where(v => v.IsArgument))
        {
            IParameterType parameterType = variable.GetParameterType();
            IStepParameter parameter = new StepParameter<IParameterType>(variable.Name, variable.Unit, parameterType);
            _arguments.Add(parameter);
        }
    }
}
