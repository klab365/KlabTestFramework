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

        foreach (ParameterData parameterData in stepData.Parameters)
        {
            IParameter? parameter = step.GetParameters().FirstOrDefault(p => p.Name == parameterData.Name);
            if (parameter is null)
            {
                throw new InvalidOperationException($"Parameter {parameterData.Name} not found in step {step.GetType().Name}");
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

        if (step is IStepWithChildren stepWithChildren)
        {
            data.Children = stepWithChildren.Children.Select(c => c.ToData()).ToList();
        }

        return data;
    }
}
