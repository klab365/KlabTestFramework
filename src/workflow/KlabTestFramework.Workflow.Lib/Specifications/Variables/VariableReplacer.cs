using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KlabTestFramework.Workflow.Lib.Specifications;

/// <summary>
/// Implementation of <see cref="IVariableReplacer"/>
/// </summary>
public class VariableReplacer : IVariableReplacer
{
    private readonly VariableFactory _variableFactory;

    public VariableReplacer(VariableFactory variableFactory)
    {
        _variableFactory = variableFactory;
    }

    public async Task ReplaceVariablesWithTheParametersAsync(Specifications.Workflow workflow)
    {
        await ReplaceStepsWithVariables(workflow.Steps, workflow.Variables);
    }

    private async Task ReplaceStepsWithVariables(IEnumerable<IStep> steps, IEnumerable<IVariable> variables)
    {
        foreach (IStep step in steps)
        {
            foreach (IParameter parameter in step.GetParameters())
            {
                if (parameter.IsValue())
                {
                    continue;
                }

                await ReplaceVariablesAsync(parameter, variables);
            }

            if (step is ISubworkflowStep subworkflowStep && subworkflowStep.Subworkflow != null)
            {
                ReplaceSubworkflowVariableWithTheArgumentsOfSubworkflowStep(subworkflowStep);
                await ReplaceStepsWithVariables(subworkflowStep.Children, subworkflowStep.Subworkflow.Variables);
            }
        }
    }

    private static void ReplaceSubworkflowVariableWithTheArgumentsOfSubworkflowStep(ISubworkflowStep subworkflowStep)
    {
        Specifications.Workflow subworkflow = subworkflowStep.Subworkflow!;
        foreach (IParameter parameter in subworkflowStep.Arguments)
        {
            IVariable subworkflowVariable = subworkflow.Variables.Single(v => v.VariableType == VariableType.Argument && v.Name == parameter.Name);
            subworkflowVariable.UpdateValue(parameter.ContentAsString());
        }
    }

    private async Task ReplaceVariablesAsync(IParameter parameter, IEnumerable<IVariable> variables)
    {
        string variableName = parameter.VariableName;
        if (!ContainVariable(variables, variableName))
        {
            throw new InvalidOperationException($"Variable '{variableName}' not found in context");
        }

        // find correct variable handler and store it in cache (faster resolving later)
        IVariableParameterReplaceHandler variableHandler = _variableFactory.CreateVariableReplaceHandler(parameter.ParameterContentType);
        IVariable variable = variables.Single(v => v.Name == variableName);
        await variableHandler.ReplaceAsync(variable, parameter);

    }

    private static bool ContainVariable(IEnumerable<IVariable> variables, string variableName)
    {
        return variables.Any(v => v.Name == variableName);
    }
}
