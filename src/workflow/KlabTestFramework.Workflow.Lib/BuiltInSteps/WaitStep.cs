using System;
using System.Collections.Generic;
using KlabTestFramework.Workflow.Lib.Specifications;
using KlabTestFramework.Workflow.Lib.Specifications.Parameters;

namespace KlabTestFramework.Workflow.Lib.BuildInSteps;

/// <summary>
/// Represents a step that waits for a specified amount of time.
/// </summary>
public class WaitStep : IStep
{
    /// <summary>
    /// Gets or sets the time to wait.
    /// </summary>
    public Parameter<TimeParameter> Time { get; }

    public WaitStep(IParameterFactory parameterFactory)
    {
        Time = parameterFactory.CreateParameter<TimeParameter>
        (
            "Time",
            "sec",
            p => p.SetValue(TimeSpan.Zero),
            p => p.AddValiation(v => v >= TimeSpan.Zero)
        );
    }

    public IEnumerable<ParameterContainer> GetParameters()
    {
        yield return new(nameof(Time), Time);
    }

    public void Init(IEnumerable<ParameterData> parameterData)
    {
        ParameterData? timeParameterData = parameterData.FoundParameterDataByName(nameof(Time));
        Time.Content.FromData(timeParameterData);
    }
}
