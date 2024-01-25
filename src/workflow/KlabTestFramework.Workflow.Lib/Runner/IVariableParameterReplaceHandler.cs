using System.Threading.Tasks;
using KlabTestFramework.Workflow.Lib.Specifications;

namespace KlabTestFramework.Workflow.Lib;

/// <summary>
/// Interface to replace variable value of a parameter
/// This interface can have multiple implementations for different types of parameters
/// </summary>
public interface IVariableParameterReplaceHandler
{
    Task ReplaceAsync(object variable, IParameter parameter);
}

/// <summary>
/// Generic interface to replace variable of a parameter
/// </summary>
/// <typeparam name="TInput"></typeparam>
public interface IVariableParameterReplaceHandler<TInput> : IVariableParameterReplaceHandler where TInput : IParameterType
{
    Task Replace(Variable<TInput> variable, Parameter<TInput> parameter);

    /// <inheritdoc/>
    Task IVariableParameterReplaceHandler.ReplaceAsync(object variable, IParameter parameter)
    {
        return Replace((Variable<TInput>)variable, (Parameter<TInput>)parameter);
    }
}
