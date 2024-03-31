using System.Threading.Tasks;
using KlabTestFramework.Shared.Parameters;
using KlabTestFramework.Workflow.Lib.Specifications;

namespace KlabTestFramework.Workflow.Lib.BuiltIn;

/// <summary>
/// Default variable handler, if no other handler is applicable
/// This handler will replace the parameter with the variable content
/// </summary>
/// <typeparam name="TParameterType"></typeparam>
public class DefaultVariableParameterReplace<TParameterType> : IVariableParameterReplaceHandler<TParameterType> where TParameterType : IParameterType
{
    public Task Replace(Variable<TParameterType> variable, Parameter<TParameterType> parameter)
    {
        string value = variable.Parameter.AsString();
        parameter.Content.FromString(value);
        return Task.CompletedTask;
    }
}
