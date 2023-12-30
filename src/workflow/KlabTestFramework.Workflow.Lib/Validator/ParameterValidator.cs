using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Klab.Toolkit.Results;
using KlabTestFramework.Workflow.Lib.Contracts;

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
        IEnumerable<IParameter> paramters = step.GetParameters();
        foreach (IParameter parameter in paramters)
        {
            if (!parameter.IsValid())
            {
                results.Add(new WorkflowStepValidateResult(id, step, new Error(1, $"Parameter {parameter.DisplayName} is not valid")));
            }
        }

        return Task.FromResult(results.AsEnumerable());
    }
}


