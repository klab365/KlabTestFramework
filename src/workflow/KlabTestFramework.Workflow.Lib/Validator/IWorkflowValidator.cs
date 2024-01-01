using System.Threading.Tasks;
using KlabTestFramework.Workflow.Lib.Specifications;

namespace KlabTestFramework.Workflow.Lib.Validator;

/// <summary>
/// Represents an interface for validating workflows.
/// </summary>
public interface IWorkflowValidator
{
    /// <summary>
    /// Validates the specified workflow asynchronously.
    /// </summary>
    /// <param name="workflow">The workflow to validate.</param>
    /// <returns>A task that represents the asynchronous validation operation.</returns>
    Task<WorkflowValidatorResult> ValidateAsync(IWorkflow workflow);
}
