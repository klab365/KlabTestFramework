using System;
using System.Collections.Generic;
using System.Linq;

namespace KlabTestFramework.Workflow.Lib.Specifications;

/// <summary>
/// Implementation of the <see cref="IParameterFactory" interface/>
/// </summary>
public class ParameterFactory : IParameterFactory
{
    private readonly IEnumerable<ParameterDependencySpecification> _parameterSpecifications;

    public IEnumerable<ParameterDependencySpecification> ParameterSpecifications => _parameterSpecifications;

    public ParameterFactory(IEnumerable<ParameterDependencySpecification> parameterSpecifications)
    {
        _parameterSpecifications = parameterSpecifications;
    }

    /// <inheritdoc/>
    public Parameter<TParameter> CreateParameter<TParameter>(string name, string unit, params Action<TParameter>[] configureCallbacks) where TParameter : IParameterType
    {
        string parameterType = typeof(TParameter).Name;
        TParameter parameter = (TParameter)CreateParameterTypeByName(parameterType);
        foreach (Action<TParameter> configureCallback in configureCallbacks)
        {
            configureCallback(parameter);
        }

        return new Parameter<TParameter>(name, unit, parameter);
    }

    /// <inheritdoc/>
    public TParameter CreateParameterType<TParameter>() where TParameter : IParameterType
    {
        string parameterType = typeof(TParameter).Name;
        return (TParameter)CreateParameterTypeByName(parameterType);
    }

    /// <inheritdoc/>
    public IParameterType CreateParameterTypeByName(string parameterType)
    {
        ParameterDependencySpecification foundParameterSpecifications = FindParameterSpecification(parameterType);
        return foundParameterSpecifications.Factory();
    }

    private ParameterDependencySpecification FindParameterSpecification(string parameterType)
    {
        ParameterDependencySpecification? foundParameterSpecifications = _parameterSpecifications.SingleOrDefault(p => p.Type.Name == parameterType);
        if (foundParameterSpecifications is null)
        {
            throw new ArgumentException($"Parameter type {parameterType} is not supported.");
        }

        return foundParameterSpecifications;
    }
}

/// <summary>
/// Class which represents a parameter dependency specification which can be used to create a parameter
/// </summary>
public class ParameterDependencySpecification
{
    public Type Type { get; }

    public Func<IParameterType> Factory { get; }

    public string Kind => Type.Name;

    public ParameterDependencySpecification(Type type, Func<IParameterType> factory)
    {
        Type = type;
        Factory = factory;
    }
}
