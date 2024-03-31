using System;
using System.Collections.Generic;
using KlabTestFramework.Shared.Parameters;
using KlabTestFramework.Shared.Parameters.Types;
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
    public Parameter<TimeParameter> Time { get; }

    public WaitStep(ParameterFactory parameterFactory)
    {
        Time = parameterFactory.CreateParameter<TimeParameter>
        (
            "Time",
            "sec",
            p => p.SetValue(TimeSpan.Zero),
            p => p.AddValidation(v => v >= TimeSpan.Zero)
        );
    }

    public IEnumerable<IParameter> GetParameters()
    {
        yield return Time;
    }
}
