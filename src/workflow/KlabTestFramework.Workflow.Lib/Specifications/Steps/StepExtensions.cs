using System.Collections.Generic;

namespace KlabTestFramework.Workflow.Lib.Specifications;

public static class StepExtensions
{
    public static void FromData(this IStep step, StepData stepData)
    {
        if (stepData.Parameters is null)
        {
            return;
        }

        if (string.IsNullOrEmpty(stepData.Id))
        {
            stepData.Id = string.Empty;
        }
        step.Id = StepId.Create(stepData.Id);

        foreach (IParameter parameter in step.GetParameters())
        {
            ParameterData parameterData = stepData.Parameters.FoundParameterDataByName(parameter.Name);
            parameter.FromData(parameterData);
        }
    }

    public static StepData ToData(this IStep step)
    {
        List<ParameterData> parameters = new();
        foreach (IParameter parameter in step.GetParameters())
        {
            parameters.Add(parameter.ToData());
        }

        StepData data = new()
        {
            Id = step.Id.IsEmpty ? string.Empty : step.Id.Value,
            Type = step.GetType().Name,
            Parameters = parameters,
        };

        return data;
    }
}
