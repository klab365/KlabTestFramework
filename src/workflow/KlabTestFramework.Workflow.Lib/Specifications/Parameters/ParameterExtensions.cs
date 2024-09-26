using System;
using System.Collections.Generic;
using System.Linq;
using KlabTestFramework.Shared.Parameters;

namespace KlabTestFramework.Workflow.Lib.Specifications;

/// <summary>
/// Extension methods for working with <see cref="ParameterData"/>.
/// </summary>
internal static class ParameterExtensions
{
    /// <summary>
    /// Finds a <see cref="ParameterData"/> object by its name in the specified collection of <see cref="ParameterData"/>.
    /// </summary>
    /// <param name="parameterData">The collection of <see cref="ParameterData"/>.</param>
    /// <param name="name">The name of the parameter to find.</param>
    /// <returns>The <see cref="ParameterData"/> object with the specified name.</returns>
    /// <exception cref="ArgumentException">Thrown when the parameter with the specified name is not found.</exception>
    public static ParameterData FoundParameterDataByName(this IEnumerable<ParameterData> parameterData, string name)
    {
        ParameterData? foundData = parameterData.SingleOrDefault(p => p.Name == name);
        if (foundData is null)
        {
            throw new ArgumentException($"Parameter {name} is missing.");
        }

        return foundData;
    }

    /// <summary>
    /// Creates a new <see cref="Parameter{TParameter}"/> object with the specified name, unit, and configuration callbacks.
    /// </summary>
    /// <typeparam name="TParameter"></typeparam>
    /// <param name="parameterFactory"></param>
    /// <param name="name"></param>
    /// <param name="unit"></param>
    /// <param name="configureCallbacks"></param>
    /// <returns></returns>
    public static Parameter<TParameter> CreateParameter<TParameter>(this ParameterFactory parameterFactory, string name, string unit, params Action<TParameter>[] configureCallbacks) where TParameter : IParameterType
    {
        TParameter parameter = parameterFactory.CreateParameterType<TParameter>();
        foreach (Action<TParameter> configureCallback in configureCallbacks)
        {
            configureCallback(parameter);
        }

        return new Parameter<TParameter>(name, unit, parameter);
    }

    /// <inheritdoc/>
    public static ParameterData ToData(this IParameter parameter)
    {
        ParameterData data = new()
        {
            Name = parameter.Name,
            Type = parameter.ParameterType,
            Value = parameter.ContentAsString()
        };

        return data;
    }

    /// <inheritdoc/>
    public static void FromData(this IParameter parameter, ParameterData data)
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
