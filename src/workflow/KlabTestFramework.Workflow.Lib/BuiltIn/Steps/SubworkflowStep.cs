using System.Collections.Generic;
using System.Linq;
using KlabTestFramework.Shared.Parameters;
using KlabTestFramework.Shared.Parameters.Types;
using KlabTestFramework.Workflow.Lib.Specifications;


namespace KlabTestFramework.Workflow.Lib.BuiltIn;

internal class SubworkflowStep : ISubworkflowStep
{
    public static readonly StringParameter NoneSelected = new() { Name = "none" };
    public StepId Id { get; set; } = StepId.Empty;

    public Parameter<SelectableParameter<StringParameter>> SelectedSubworkflow { get; }

    public List<IStep> Children { get; private set; } = new();

    public List<IParameter> Arguments { get; private set; } = new();

    public Specifications.Workflow Subworkflow { get; } = new(); 

    public SubworkflowStep(ParameterFactory parameterFactory, StepFactory stepFactory)
    {
        SelectedSubworkflow = parameterFactory.CreateParameter<SelectableParameter<StringParameter>>
        (
            "Subworkflow",
            string.Empty,
            p => p.SetValue(NoneSelected)
        );
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

    public void SelectWorkflow(WorkflowData wfData)
    {
        


    }
}
