using System;
using System.Collections.Generic;
using System.Linq;

namespace KlabTestFramework.Workflow.Lib.Specifications;

internal static class StepExtensions
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


        foreach ((int index, IParameter parameter) in step.GetParameters().Select((parameter, index) => (index, parameter)))
        {
            ParameterData? parameterData = stepData.Parameters.ElementAtOrDefault(index);
            if (parameterData is null)
            {
                continue; // No data for this parameter
            }

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
