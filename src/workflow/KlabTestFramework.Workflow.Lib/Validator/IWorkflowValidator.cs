using System.Threading.Tasks;
using Klab.Toolkit.Results;

namespace KlabTestFramework.Workflow.Lib.BuildInSteps;

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
    Task<Result<WorkflowValidatorResult>> ValidateAsync(Specifications.Workflow workflow);
}
