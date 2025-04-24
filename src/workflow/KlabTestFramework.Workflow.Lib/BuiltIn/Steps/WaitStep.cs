using System.Collections.Generic;
using KlabTestFramework.Shared.Parameters;
using KlabTestFramework.Shared.Parameters.Types;
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
            p => p.SetValue(0),
            p => p.AddValidation(v => v > 0)
        );
    }

    public IEnumerable<IStepParameter> GetParameters()
    {
        yield return Time;
    }
}
