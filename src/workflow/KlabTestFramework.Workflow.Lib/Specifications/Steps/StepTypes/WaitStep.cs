using System;
using System.Collections.Generic;
using KlabTestFramework.Workflow.Lib.Contracts;
using KlabTestFramework.Workflow.Lib.Specifications.Parameters;
using KlabTestFramework.Workflow.Lib.Specifications.Parameters.ParameterTypes;

namespace KlabTestFramework.Workflow.Lib.Specifications.Steps.StepTypes;

/// <summary>
/// Represents a step that waits for a specified amount of time.
/// </summary>
public class WaitStep : IStep
{
    /// <summary>
    /// Gets or sets the time to wait.
    /// </summary>
    public SingleValueParameter<TimeSpan> Time { get; }

    /// <summary>
    /// Selected time unit variable
    /// </summary>
    /// <value></value>
    public ChoicesParameter<TimeUnit> SelectedTimeUnit { get; }

    public WaitStep(IParameterFactory parameterFactory)
    {
        Time = parameterFactory.CreateSingleValueParameter
        (
            "Time",
            "sec",
            TimeSpan.Zero,
            t => t > TimeSpan.Zero
        );

        SelectedTimeUnit = parameterFactory.CreateChoicesParameter
        (
            "Available times",
            "sec",
            TimeUnit.All
        );
    }

    public IEnumerable<IParameter> GetParameters()
    {
        yield return Time;
        yield return SelectedTimeUnit;
    }
}

public record TimeUnit
{
    public string Unit { get; }

    public static TimeUnit Seconds => new("sec");

    public static TimeUnit Minutes => new("min");

    public static TimeUnit Hours => new("h");

    public static TimeUnit[] All => [Seconds, Minutes, Hours];


    private TimeUnit(string unit)
    {
        Unit = unit;
    }
}
