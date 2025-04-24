using System;
using System.Collections.Generic;
using System.Linq;
using KlabTestFramework.Shared.Parameters;
using KlabTestFramework.Workflow.Abstractions.Specifications;

namespace KlabTestFramework.Workflow.Lib.Specifications;

/// <summary>
/// Extension methods for working with <see cref="StepParameterData"/>.
/// </summary>
internal static class StepParameterExtensions
{
    /// <summary>
    /// Finds a <see cref="StepParameterData"/> object by its name in the specified collection of <see cref="StepParameterData"/>.
    /// </summary>
    /// <param name="parameterData">The collection of <see cref="StepParameterData"/>.</param>
    /// <param name="name">The name of the parameter to find.</param>
    /// <returns>The <see cref="StepParameterData"/> object with the specified name.</returns>
    /// <exception cref="ArgumentException">Thrown when the parameter with the specified name is not found.</exception>
    public static StepParameterData FoundParameterDataByName(this IEnumerable<StepParameterData> parameterData, string name)
    {
        StepParameterData? foundData = parameterData.SingleOrDefault(p => p.Name == name);
        if (foundData is null)
        {
            throw new ArgumentException($"Parameter {name} is missing.");
        }

        return foundData;
    }

    /// <summary>
    /// Creates a new <see cref="StepParameter{TParameter}"/> object with the specified name, unit, and configuration callbacks.
    /// </summary>
    /// <typeparam name="TParameter"></typeparam>
    /// <param name="parameterFactory"></param>
    /// <param name="name"></param>
    /// <param name="unit"></param>
    /// <param name="configureCallbacks"></param>
    /// <returns></returns>
    public static StepParameter<TParameter> CreateParameter<TParameter>(this ParameterFactory parameterFactory, string name, string unit, params Action<TParameter>[] configureCallbacks) where TParameter : IParameterType
    {
        TParameter parameter = parameterFactory.CreateParameterType<TParameter>();
        foreach (Action<TParameter> configureCallback in configureCallbacks)
        {
            configureCallback(parameter);
        }

        return new StepParameter<TParameter>(name, unit, parameter);
    }

    /// <inheritdoc/>
    public static StepParameterData ToData(this IStepParameter parameter)
    {
        StepParameterData data = new()
        {
            Name = parameter.Name,
            Type = parameter.ParameterType,
            Value = parameter.ContentAsString()
        };

        return data;
    }

    /// <inheritdoc/>
    public static void FromData(this IStepParameter parameter, StepParameterData data)
    {
        parameter.Name = data.Name;
        parameter.ParameterType = data.Type;

        if (parameter.IsVariable())
        {
            parameter.VariableName = data.Value;
        }
        else
        {
            parameter.GetParameterType().FromString(data.Value);
        }
    }
}
