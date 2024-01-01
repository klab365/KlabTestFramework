using System.Collections.Generic;


namespace KlabTestFramework.Workflow.Lib.Validator;

/// <summary>
/// Represents the result of a workflow validation.
/// </summary>
public class WorkflowValidatorResult
{
    /// <summary>
    /// Gets the list of workflow step validation errors.
    /// </summary>
    public List<WorkflowStepErrorValidation> Errors { get; } = new();

    /// <summary>
    /// Adds a collection of errors to the result.
    /// </summary>
    /// <param name="errors">The errors to add.</param>
    public void AddErrors(IEnumerable<WorkflowStepErrorValidation> errors)
    {
        Errors.AddRange(errors);
    }
}
