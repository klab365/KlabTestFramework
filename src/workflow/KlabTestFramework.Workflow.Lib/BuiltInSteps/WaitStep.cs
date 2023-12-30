using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using KlabTestFramework.Workflow.Lib.Contracts;
using KlabTestFramework.Workflow.Lib.Specifications;

namespace KlabTestFramework.Workflow.Lib.BuildInSteps;

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

    public IEnumerable<ParameterData>? GetParameterData()
    {
        List<ParameterData> parameterData = new()
        {
            new ParameterData(){Name = nameof(Time), Value = Time.Value!.ToString()}
        };

        return parameterData;
    }

    public void Init(IEnumerable<ParameterData> parameterData)
    {
        ParameterData? timeParameterData = parameterData.SingleOrDefault(p => p.Name == nameof(Time));
        if (timeParameterData is not null)
        {
            Time.SetValue(TimeSpan.Parse(timeParameterData.Value!, CultureInfo.InvariantCulture));
        }

        ParameterData? selectedTimeUnitParameterData = parameterData.SingleOrDefault(p => p.Name == nameof(SelectedTimeUnit));
        if (selectedTimeUnitParameterData is not null)
        {
            SelectedTimeUnit.SetValue(TimeUnit.All.Single(t => t.Unit == selectedTimeUnitParameterData.Value));
        }
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
