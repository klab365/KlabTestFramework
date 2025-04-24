using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KlabTestFramework.Shared.Parameters;
using KlabTestFramework.Shared.Parameters.Types;
using KlabTestFramework.Workflow.Abstractions.Features.Validator;
using KlabTestFramework.Workflow.Abstractions.Specifications;
using KlabTestFramework.Workflow.Lib.Specifications;

namespace KlabTestFramework.Workflow.Lib.BuiltIn;

/// <summary>
/// Represents a step that waits for a specified amount of time.
/// </summary>
public class WaitStep : IStep
{
    public StepId Id { get; set; } = StepId.Empty;

    /// <summary>
    /// Gets or sets the time to wait.
    /// </summary>
    public StepParameter<IntParameter> Time { get; }

    public WaitStep(ParameterFactory parameterFactory)
    {
        Time = parameterFactory.CreateParameter<IntParameter>
        (
            "Time",
            "sec",
            p => p.SetValue(0)
        );
    }

    public IEnumerable<IStepParameter> GetParameters()
    {
        yield return Time;
    }

    public Task<WorkflowStepErrorValidation[]> ValidateAsync(CancellationToken cancellationToken = default)
    {
        var errors = new List<WorkflowStepErrorValidation>();
        if (Time.Content.Value < 0)
        {
            errors.Add(new WorkflowStepErrorValidation(this, "Time must be greater than or equal to 0."));
        }

        return Task.FromResult(errors.ToArray());
    }
}
