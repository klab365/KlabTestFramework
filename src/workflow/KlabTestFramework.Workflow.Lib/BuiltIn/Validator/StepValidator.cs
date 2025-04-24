using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KlabTestFramework.Workflow.Abstractions.Features.Validator;
using KlabTestFramework.Workflow.Abstractions.Specifications;

namespace KlabTestFramework.Workflow.Lib.BuiltIn.Validator;

/// <summary>
/// Validate if the step is valid
/// </summary>
public class StepValidator : IStepValidatorHandler
{
    /// <inheritdoc/>
    public async Task<IEnumerable<WorkflowStepErrorValidation>> ValidateAsync(IStep step, CancellationToken cancellationToken = default)
    {
        return await step.ValidateAsync(cancellationToken);
    }
}


