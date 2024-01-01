using System;
using System.Collections.Generic;

namespace KlabTestFramework.Workflow.Lib.Specifications;

/// <summary>
/// Factory for the parameter creation
/// </summary>
public interface IParameterFactory
{
    /// <summary>
    /// List of parameter specifications
    /// </summary>
    IEnumerable<ParameterDependencySpecification> ParameterSpecifications { get; }

    /// <summary>
    /// Create parameter type by name
    /// </summary>
    IParameterType CreateParameterTypeByName(string parameterType);

    /// <summary>
    /// Create a parameter type
    /// </summary>
    /// <typeparam name="TParameter"></typeparam>
    /// <returns></returns>
    TParameter CreateParameterType<TParameter>() where TParameter : IParameterType;

    /// <summary>
    /// Create a parameter
    /// </summary>
    /// <returns></returns>
    Parameter<TParameter> CreateParameter<TParameter>(string name, string unit, params Action<TParameter>[] configureCallbacks) where TParameter : IParameterType;
}
