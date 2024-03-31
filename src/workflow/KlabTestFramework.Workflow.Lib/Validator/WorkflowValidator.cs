using System.Collections.Generic;
using System.Threading.Tasks;
using KlabTestFramework.Workflow.Lib.Specifications;


namespace KlabTestFramework.Workflow.Lib.Validator;

/// <summary>
/// Implementation of <see cref="IWorkflowValidator"/>.
/// </summary>
public class WorkflowValidator : IWorkflowValidator
{
    private readonly IEnumerable<IStepValidatorHandler> _stepValidatorHandlers;

    public WorkflowValidator(IEnumerable<IStepValidatorHandler> stepValidatorHandlers)
    {
        _stepValidatorHandlers = stepValidatorHandlers;
    }

    /// <inheritdoc/>
    public async Task<WorkflowValidatorResult> ValidateAsync(IWorkflow workflow)
    {
        WorkflowValidatorResult result = new();
        foreach (IStep step in workflow.Steps)
        {
            foreach (IStepValidatorHandler stepValidatorHandler in _stepValidatorHandlers)
            {
                IEnumerable<WorkflowStepErrorValidation> stepValidations = await stepValidatorHandler.ValidateAsync(step);
                result.AddErrors(stepValidations);
            }
        }

        return result;
    }
}
