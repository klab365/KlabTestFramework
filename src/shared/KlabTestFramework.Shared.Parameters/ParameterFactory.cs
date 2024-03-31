using System;
using System.Collections.Generic;
using System.Linq;

namespace KlabTestFramework.Shared.Parameters;

/// <summary>
/// Factory for the parameter creation
/// </summary>
public class ParameterFactory
{
    private readonly IEnumerable<ParameterDependencySpecification> _parameterSpecifications;

    public ParameterFactory(IEnumerable<ParameterDependencySpecification> parameterSpecifications)
    {
        _parameterSpecifications = parameterSpecifications;
    }

    /// <inheritdoc/>
    public TParameter CreateParameterType<TParameter>() where TParameter : IParameterType
    {
        Type parameterType = typeof(TParameter);
        ParameterDependencySpecification? parameterSpecifications = _parameterSpecifications.SingleOrDefault(x => x.Type == parameterType);
        if (parameterSpecifications is null)
        {
            throw new InvalidOperationException($"Parameter {parameterType.Name} not found");
        }

        return (TParameter)parameterSpecifications.Factory();
    }

    public IParameterType CreateParameterTypeByName(string parameterTypeName)
    {
        ParameterDependencySpecification? parameterSpecifications = _parameterSpecifications.SingleOrDefault(p => p.Key == parameterTypeName);
        if (parameterSpecifications is null)
        {
            throw new InvalidOperationException($"Parameter {parameterTypeName} not found");
        }

        return parameterSpecifications.Factory();
    }
}

/// <summary>
/// Class which represents a parameter dependency specification which can be used to create a parameter
/// </summary>
public class ParameterDependencySpecification
{
    public string Key { get; }
    public Type Type { get; }
    public Func<IParameterType> Factory { get; }

    public ParameterDependencySpecification(string key, Type type, Func<IParameterType> factory)
    {
        Key = key;
        Type = type;
        Factory = factory;
    }
}
