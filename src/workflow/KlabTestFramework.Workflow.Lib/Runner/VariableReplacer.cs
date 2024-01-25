using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KlabTestFramework.Workflow.Lib.Specifications;
using Microsoft.Extensions.DependencyInjection;

namespace KlabTestFramework.Workflow.Lib;

/// <summary>
/// Implementation of <see cref="IVariableReplacer"/>
/// </summary>
public class VariableReplacer : IVariableReplacer
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ConcurrentDictionary<Type, IVariableParameterReplaceHandler> _variableHandlers = new();

    public VariableReplacer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task ReplaceVariablesWithTheParametersAsync(IWorkflow workflow)
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
        IWorkflow subworkflow = subworkflowStep.Subworkflow!;
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
        IVariableParameterReplaceHandler variableHandler = _variableHandlers.GetOrAdd(parameter.ParameterContentType, requestType =>
        {
            Type genericVariableHandlerType = typeof(IVariableParameterReplaceHandler<>).MakeGenericType(requestType);
            object? foundVariableHandler = _serviceProvider.GetService(genericVariableHandlerType);
            if (foundVariableHandler is null)
            {
                Type defaultVariableHandlerType = typeof(DefaultVariableParameterReplace<>).MakeGenericType(requestType);
                return (IVariableParameterReplaceHandler)_serviceProvider.GetRequiredService(defaultVariableHandlerType);
            }

            return (IVariableParameterReplaceHandler)foundVariableHandler;
        });

        IVariable variable = variables.Single(v => v.Name == variableName);
        await variableHandler.ReplaceAsync(variable, parameter);

    }

    private static bool ContainVariable(IEnumerable<IVariable> variables, string variableName)
    {
        return variables.Any(v => v.Name == variableName);
    }
}
