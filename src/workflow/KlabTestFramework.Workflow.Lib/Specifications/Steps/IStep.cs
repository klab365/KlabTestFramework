using System;
using System.Collections.Generic;
using KlabTestFramework.Workflow.Lib.BuiltIn;

namespace KlabTestFramework.Workflow.Lib.Specifications;

/// <summary>
/// Represents a step in a workflow.
/// </summary>
public interface IStep
{
    Guid Id { get; }

    /// <summary>
    /// Get the parameters of the step.
    /// </summary>
    /// <returns></returns>
    IEnumerable<IParameter> GetParameters();
}

/// <summary>
/// Represents a step in a workflow that has children.
/// </summary>
public interface IStepWithChildren : IStep
{
    /// <summary>
    /// Get the children of the step.
    /// </summary>
    /// <returns></returns>
    List<IStep> Children { get; }
}

/// <summary>
/// Represents a step in a workflow that is a subworkflow.
/// </summary>
public interface ISubworkflowStep : IStepWithChildren
{
    event EventHandler<string>? SubworkflowSelected;

    Parameter<StringParameter> SelectedSubworkflow { get; }

    /// <summary>
    /// Get the subworkflow of the step.
    /// </summary>
    /// <returns></returns>
    IWorkflow? Subworkflow { get; set; }

    List<IParameter> Arguments { get; }
}

public static class StepExtensions
{
    public static void FromData(this IStep step, StepData stepData)
    {
        if (stepData.Parameters is null)
        {
            return;
        }

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

        StepData data = new() { Type = step.GetType().Name, Parameters = parameters };
        return data;
    }
}
