using System.Threading.Tasks;

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
