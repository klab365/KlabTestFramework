using System.Collections.Generic;
using System.Threading.Tasks;
using Klab.Toolkit.Results;
using KlabTestFramework.Workflow.Lib.Contracts;

namespace KlabTestFramework.Workflow.Lib.BuildInSteps;

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
    public async Task<Result<WorkflowValidatorResult>> ValidateAsync(Specifications.Workflow workflow)
    {
        WorkflowValidatorResult result = new();
        foreach (Specifications.StepContainer step in workflow.Steps)
        {
            foreach (IStepValidatorHandler stepValidatorHandler in _stepValidatorHandlers)
            {
                IEnumerable<WorkflowStepValidateResult> stepValidations = await stepValidatorHandler.ValidateAsync(step.Id, step.Step);
                result.AddErrors(stepValidations);
            }
        }

        return result;
    }
}
