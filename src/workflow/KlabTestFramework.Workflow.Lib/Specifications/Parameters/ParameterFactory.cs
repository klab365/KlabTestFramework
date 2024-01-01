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

    public ParameterFactory(IEnumerable<ParameterDependencySpecification> parameterSpecifications)
    {
        _parameterSpecifications = parameterSpecifications;
    }

    /// <inheritdoc/>
    public Parameter<TParameter> CreateParameter<TParameter>(string displayName, string unit, params Action<TParameter>[] configureCallbacks) where TParameter : IParameterType
    {
        ParameterDependencySpecification foundParameterSpecifications = FindParameterSpecification<TParameter>();
        TParameter parameter = (TParameter)foundParameterSpecifications.Factory();
        foreach (Action<TParameter> configureCallback in configureCallbacks)
        {
            configureCallback(parameter);
        }

        return new Parameter<TParameter>(displayName, unit, parameter);
    }

    public TParameter CreateParameterType<TParameter>() where TParameter : IParameterType
    {
        ParameterDependencySpecification? foundParameterSpecifications = FindParameterSpecification<TParameter>();
        return (TParameter)foundParameterSpecifications.Factory();
    }

    private ParameterDependencySpecification FindParameterSpecification<TParameter>() where TParameter : IParameterType
    {
        ParameterDependencySpecification? foundParameterSpecifications = _parameterSpecifications.SingleOrDefault(p => p.Type == typeof(TParameter));
        if (foundParameterSpecifications is null)
        {
            throw new ArgumentException($"Parameter type {typeof(TParameter).Name} is not supported.");
        }

        return foundParameterSpecifications;
    }
}

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
