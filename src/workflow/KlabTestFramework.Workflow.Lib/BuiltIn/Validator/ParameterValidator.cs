using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KlabTestFramework.Workflow.Lib.Features.Validator;
using KlabTestFramework.Workflow.Lib.Specifications;

namespace KlabTestFramework.Workflow.Lib.BuiltIn.Validator;

/// <summary>
/// Validate if the parameters of a step are valid
/// </summary>
public class ParameterValidator : IStepValidatorHandler
{
    /// <inheritdoc/>
    public Task<IEnumerable<WorkflowStepErrorValidation>> ValidateAsync(IStep step)
    {
        List<WorkflowStepErrorValidation> results = new();
        IEnumerable<IParameter> paramters = step.GetParameters();
        foreach (IParameter parameter in paramters)
        {
            if (!parameter.IsValid())
            {
                results.Add(new(step, $"Parameter {parameter.Name} is not valid"));
            }
        }

        return Task.FromResult(results.AsEnumerable());
    }
}


