using System.Threading.Tasks;

namespace KlabTestFramework.Workflow.Lib.Specifications;

/// <summary>
/// Interface to replace variables in a step
/// </summary>
public interface IVariableReplacer
{
    /// <summary>
    /// Replace variables
    /// </summary>
    /// <param name="workflow"></param>
    /// <returns></returns>
    Task ReplaceVariablesWithTheParametersAsync(IWorkflow workflow);
}
