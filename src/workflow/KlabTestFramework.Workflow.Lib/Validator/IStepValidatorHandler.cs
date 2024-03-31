using System.Collections.Generic;
using System.Threading.Tasks;
using KlabTestFramework.Workflow.Lib.Specifications;

namespace KlabTestFramework.Workflow.Lib.Validator;

/// <summary>
/// Represents a handler for validating workflow steps.
/// </summary>
public interface IStepValidatorHandler
{
    /// <summary>
    /// Validates a workflow step asynchronously.
    /// </summary>
    /// <param name="step">The step to validate.</param>
    /// <returns>A collection of validation results for the step.</returns>
    Task<IEnumerable<WorkflowStepErrorValidation>> ValidateAsync(IStep step);
}

/// <summary>
/// Represents the result of validating a workflow step.
/// </summary>
/// <param name="Step">The workflow step being validated.</param>
/// <param name="Result">The validation result.</param>
public record WorkflowStepErrorValidation(IStep Step, string ErrorMessage);
