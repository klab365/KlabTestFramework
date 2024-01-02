using System.Threading.Tasks;
using KlabTestFramework.Workflow.Lib.Specifications;

namespace KlabTestFramework.Workflow.Lib;

/// <summary>
/// Interface to replace variables in a step
/// </summary>
public interface IVariableParameterReplaceHandler
{
    Task ReplaceAsync(object variable, IParameter parameter);
}

public interface IVariableParameterReplaceHandler<TInput> : IVariableParameterReplaceHandler where TInput : IParameterType
{
    Task Replace(Variable<TInput> variable, Parameter<TInput> parameter);

    /// <inheritdoc/>
    Task IVariableParameterReplaceHandler.ReplaceAsync(object variable, IParameter parameter)
    {
        return Replace((Variable<TInput>)variable, (Parameter<TInput>)parameter);
    }
}

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

