using System;
using System.Collections.Generic;
using System.Linq;
using KlabTestFramework.Shared.Parameters;
using KlabTestFramework.Shared.Parameters.Types;
using KlabTestFramework.Workflow.Lib.Specifications;


namespace KlabTestFramework.Workflow.Lib.BuiltIn;

public class SubworkflowStep : ISubworkflowStep
{
    public static readonly StringParameter NoneSelected = new() { Name = "none" };
    private readonly StepFactory _stepFactory;
    public StepId Id { get; set; } = StepId.Empty;

    public Parameter<SelectableParameter<StringParameter>> SelectedSubworkflow { get; }

    public List<IStep> Children { get; private set; } = new();

    public List<IParameter> Arguments { get; private set; } = new();

    private IWorkflow? _subworkflow;
    public IWorkflow? Subworkflow
    {
        get => _subworkflow;
        set
        {
            _subworkflow = value;
            InitInternalStructure();
        }
    }

    public event Action<string>? SubworkflowSelected;

    public SubworkflowStep(StepFactory stepFactory, ParameterFactory parameterFactory)
    {
        _stepFactory = stepFactory;
        SelectedSubworkflow = parameterFactory.CreateParameter<SelectableParameter<StringParameter>>
        (
            "Subworkflow",
            string.Empty,
            p => p.SetValue(NoneSelected)
        );

        SelectedSubworkflow.Content.ValueChanged += OnSelectedSubworkflowChanged;
    }

    public IEnumerable<IParameter> GetParameters()
    {
        yield return SelectedSubworkflow;
        foreach (IParameter argument in Arguments)
        {
            yield return argument;
        }
    }

    public void ReplaceArgumentValue(string argumentName, string value)
    {
        IParameter argument = Arguments.Single(a => a.Name == argumentName);
        argument.GetParameterType().FromString(value);
    }

    public void SelectSubworkflow(string subWorkflow)
    {
        StringParameter parameter = new();
        parameter.SetValue(subWorkflow);
        SelectedSubworkflow.Content.SelectOption(parameter);
    }

    private void OnSelectedSubworkflowChanged(StringParameter parameter)
    {
        SubworkflowSelected?.Invoke(parameter.Value);
    }

    private List<IStep> GetChildrenOfSelectedSubworkflow()
    {
        List<IStep> steps = new();
        if (Subworkflow == null)
        {
            return steps;
        }

        foreach (IStep step in Subworkflow.Steps)
        {
            StepData data = step.ToData();
            IStep clonedStep = _stepFactory.CreateStep(data);
            clonedStep.FromData(data);
            steps.Add(clonedStep);
        }

        return steps;
    }

    private List<IParameter> GetArgumentsOfSelectedSubworkflow()
    {
        List<IParameter> arguments = new();
        if (Subworkflow == null)
        {
            return arguments;
        }

        IEnumerable<IVariable> argumentVariables = Subworkflow.Variables.Where(v => v.IsArgument);
        foreach (IVariable variable in argumentVariables)
        {
            IParameterType parameterType = variable.GetParameterType().Clone();
            IParameter parameter = new Parameter<IParameterType>(variable.Name, variable.Unit, parameterType);
            arguments.Add(parameter);
        }

        return arguments;
    }

    private void InitInternalStructure()
    {
        Arguments = GetArgumentsOfSelectedSubworkflow();
        Children = GetChildrenOfSelectedSubworkflow();
    }
}
