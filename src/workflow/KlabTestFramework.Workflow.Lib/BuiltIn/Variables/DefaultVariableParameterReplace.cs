using System;
using System.Threading.Tasks;
using KlabTestFramework.Workflow.Lib.Specifications;

namespace KlabTestFramework.Workflow.Lib.BuiltIn;

/// <summary>
/// Default variable handler, if no other handler is applicable
/// This handler will replace the parameter with the variable content
/// </summary>
/// <typeparam name="TParameterType"></typeparam>
public class DefaultVariableParameterReplace : IVariableParameterReplaceHandler
{
    public Task ReplaceAsync(IVariable variable, IParameter parameter)
    {
        bool isVariableTypeEqual = variable.GetParameterType().TypeKey == parameter.GetParameterType().TypeKey;
        if (!isVariableTypeEqual)
        {
            throw new ArgumentException($"Variable type {variable.GetParameterType().TypeKey} is not equal to parameter type {parameter.GetParameterType().TypeKey}");
        }

        string value = variable.GetParameterType().AsString();
        parameter.GetParameterType().FromString(value);
        return Task.CompletedTask;
    }
}
