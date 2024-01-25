using System.Threading.Tasks;
using KlabTestFramework.Workflow.Lib.Specifications;

namespace KlabTestFramework.Workflow.Lib;

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
