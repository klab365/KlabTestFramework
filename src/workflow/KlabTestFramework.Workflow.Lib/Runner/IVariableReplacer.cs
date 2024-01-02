using System.Threading.Tasks;
using KlabTestFramework.Workflow.Lib.Specifications;

namespace KlabTestFramework.Workflow.Lib;

/// <summary>
/// Interface to replace variables in a step
/// </summary>
public interface IVariableReplacer
{
    Task ReplaceVariablesWithTheParametersAsync(IWorkflow workflow);
}
