using System;
using System.Collections.Generic;

namespace KlabTestFramework.Workflow.Lib.Specifications;

/// <summary>
/// Container for the step itself
/// </summary>
public class StepContainer
{
    public Guid Id { get; set; } = Guid.NewGuid(); // new guid for each step (then we can track the step)

    public IStep Step { get; }

    public Type StepType => Step.GetType();

    public StepContainer(IStep step)
    {
        Step = step;
    }

    public void FromData(StepData stepData)
    {
        if (stepData.Parameters is null)
        {
            return;
        }

        // nothing to do here for now
        foreach (IParameter parameter in Step.GetParameters())
        {
            ParameterData parameterData = stepData.Parameters.FoundParameterDataByName(parameter.Name);
            parameter.FromData(parameterData);
        }
    }

    public StepData ToData()
    {
        List<ParameterData> parameters = new();
        foreach (IParameter parameter in Step.GetParameters())
        {
            parameters.Add(parameter.ToData());
        }

        StepData data = new() { Type = Step.GetType().Name, Parameters = parameters };
        return data;
    }
}
