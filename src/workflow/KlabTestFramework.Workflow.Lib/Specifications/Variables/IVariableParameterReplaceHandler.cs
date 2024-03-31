using System.Threading.Tasks;
using KlabTestFramework.Shared.Parameters;

namespace KlabTestFramework.Workflow.Lib.Specifications;

/// <summary>
/// Interface to replace variable value of a parameter
/// This interface can have multiple implementations for different types of parameters
/// </summary>
public interface IVariableParameterReplaceHandler
{
    /// <summary>
    /// Generic replace method, which will accept any type of variable and parameter
    /// </summary>
    /// <param name="variable"></param>
    /// <param name="parameter"></param>
    /// <returns></returns>
    Task ReplaceAsync(IVariable variable, IParameter parameter);
}

/// <summary>
/// Generic interface to replace variable of a parameter
/// </summary>
/// <typeparam name="TInput"></typeparam>
public interface IVariableParameterReplaceHandler<TInput> : IVariableParameterReplaceHandler where TInput : IParameterType
{
    /// <summary>
    /// Typed replace method
    /// </summary>
    /// <param name="variable"></param>
    /// <param name="parameter"></param>
    /// <returns></returns>
    Task Replace(Variable<TInput> variable, Parameter<TInput> parameter);

    /// <inheritdoc/>
    Task IVariableParameterReplaceHandler.ReplaceAsync(IVariable variable, IParameter parameter)
    {
        return Replace((Variable<TInput>)variable, (Parameter<TInput>)parameter);
    }
}
