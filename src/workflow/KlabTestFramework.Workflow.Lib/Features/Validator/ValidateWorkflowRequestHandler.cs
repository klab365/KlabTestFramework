using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Klab.Toolkit.Event;
using Klab.Toolkit.Results;
using KlabTestFramework.Workflow.Lib.Specifications;


namespace KlabTestFramework.Workflow.Lib.Features.Validator;

/// <summary>
/// Implementation of <see cref=" Specifications.WorkflowValidator"/>.
/// </summary>
internal class ValidateWorkflowRequestHandler : IRequestHandler<ValidateWorkflowRequest, WorkflowValidatorResult>
{
    private readonly IEnumerable<IStepValidatorHandler> _stepValidatorHandlers;

    public ValidateWorkflowRequestHandler(IEnumerable<IStepValidatorHandler> stepValidatorHandlers)
    {
        _stepValidatorHandlers = stepValidatorHandlers;
    }

    public async Task<Result<WorkflowValidatorResult>> HandleAsync(ValidateWorkflowRequest request, CancellationToken cancellationToken)
    {
        WorkflowValidatorResult result = new();
        foreach (IStep step in request.Workflow.Steps)
        {
            await ValidateStepAsync(step, result);

            if (step is ISubworkflowStep subworkflowStep)
            {
                foreach (IStep childStep in subworkflowStep.Children)
                {
                    await ValidateStepAsync(childStep, result);
                }
            }
        }

        return Result.Success(result);
    }

    private async Task ValidateStepAsync(IStep step, WorkflowValidatorResult result)
    {
        foreach (IStepValidatorHandler stepValidatorHandler in _stepValidatorHandlers)
        {
            IEnumerable<WorkflowStepErrorValidation> stepValidations = await stepValidatorHandler.ValidateAsync(step);
            result.AddErrors(stepValidations);
        }
    }
}

public record ValidateWorkflowRequest(Specifications.Workflow Workflow) : IRequest<WorkflowValidatorResult>;

/// <summary>
/// Represents the result of a workflow validation.
/// </summary>
public class WorkflowValidatorResult
{
    public bool IsSuccess => Errors.Count == 0;

    public bool IsFailure => !IsSuccess;

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

