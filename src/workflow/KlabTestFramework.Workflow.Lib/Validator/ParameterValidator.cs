using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KlabTestFramework.Workflow.Lib.BuildInSteps;

/// <summary>
/// Validate if the parameters of a step are valid
/// </summary>
public class ParameterValidator : IStepValidatorHandler
{
    /// <inheritdoc/>
    public Task<IEnumerable<WorkflowStepValidateResult>> ValidateAsync(Guid id, IStep step)
    {
        List<WorkflowStepValidateResult> results = new();
        IEnumerable<ParameterContainer> paramters = step.GetParameters();
        foreach (ParameterContainer parameter in paramters)
        {
            // if (!parameter.IsValid())
            // {
            //     results.Add(new WorkflowStepValidateResult(id, step, new Error(1, $"Parameter {parameter.Key} is not valid")));
            // }
        }

        return Task.FromResult(results.AsEnumerable());
    }
}


