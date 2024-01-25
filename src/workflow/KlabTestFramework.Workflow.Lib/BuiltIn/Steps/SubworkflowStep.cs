using System;
using System.Collections.Generic;
using System.Linq;
using KlabTestFramework.Workflow.Lib.Specifications;


namespace KlabTestFramework.Workflow.Lib.BuiltIn;

public class SubworkflowStep : ISubworkflowStep
{
    public static readonly string NoneSelected = "none";
    private readonly IParameterFactory _parameterFactory;
    private readonly IStepFactory _stepFactory;

    public Guid Id { get; } = Guid.NewGuid();

    public Parameter<StringParameter> SelectedSubworkflow { get; }

    public event EventHandler<string>? SubworkflowSelected;

    public List<IStep> Children { get; private set; } = new();

    public List<IParameter> Arguments { get; private set; } = new();

    public IWorkflow? Subworkflow { get; set; }

    public SubworkflowStep(IStepFactory stepFactory, IParameterFactory parameterFactory)
    {
        _parameterFactory = parameterFactory;
        _stepFactory = stepFactory;
        SelectedSubworkflow = parameterFactory.CreateParameter<StringParameter>
        (
            "Subworkflow",
            string.Empty,
            p => p.SetValue(NoneSelected)
        );

        SelectedSubworkflow.Content.ValueChanged += (s) =>
        {
            SubworkflowSelected?.Invoke(this, s);
            InitInternalStructure();
        };
    }

    private void InitInternalStructure()
    {
        Arguments = GetArgumentsOfSelectedSubworkflow();
        Children = GetChildrenOfSelectedSubworkflow();
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
            IParameter parameter = _parameterFactory.CreateParameter(parameterType, variable.Name, variable.Unit);
            arguments.Add(parameter);
        }

        return arguments;
    }
}
