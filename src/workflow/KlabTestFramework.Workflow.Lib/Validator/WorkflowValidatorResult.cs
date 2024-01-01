using System.Collections.Generic;


namespace KlabTestFramework.Workflow.Lib.BuildInSteps;

/// <summary>
/// Represents the result of a workflow validation.
/// </summary>
public class WorkflowValidatorResult
{
    /// <summary>
    /// Gets the list of workflow step validation errors.
    /// </summary>
    public List<WorkflowStepValidateResult> Errors { get; } = new();

    /// <summary>
    /// Adds a collection of errors to the result.
    /// </summary>
    /// <param name="errors">The errors to add.</param>
    public void AddErrors(IEnumerable<WorkflowStepValidateResult> errors)
    {
        foreach (WorkflowStepValidateResult error in errors)
        {
            if (error.Result.IsSuccess)
            {
                continue;
            }

            Errors.Add(error);
        }
    }
}
