using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KlabTestFramework.Workflow.Lib.Specifications;
using Microsoft.Extensions.DependencyInjection;

namespace KlabTestFramework.Workflow.Lib;

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
        foreach (StepContainer step in workflow.Steps)
        {
            foreach (IParameter parameter in step.Step.GetParameters())
            {
                if (!parameter.IsVariable())
                {
                    continue;
                }

                await ReplaceVariablesAsync(parameter, workflow.Variables);
            }
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
