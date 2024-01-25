using System.Threading.Tasks;
using KlabTestFramework.Workflow.Lib.Specifications;

namespace KlabTestFramework.Workflow.Lib;

/// <summary>
/// Default variable handler, if no other handler is applicable
/// </summary>
/// <typeparam name="TParameterType"></typeparam>
public class DefaultVariableParameterReplace<TParameterType> : IVariableParameterReplaceHandler<TParameterType> where TParameterType : IParameterType
{
    public Task Replace(Variable<TParameterType> variable, Parameter<TParameterType> parameter)
    {
        parameter.Content.FromString(variable!.Parameter!.AsString());
        return Task.CompletedTask;
    }
}
