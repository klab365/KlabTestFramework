using System;

namespace KlabTestFramework.Workflow.Lib.Specifications;

/// <summary>
/// Factory for the parameter creation
/// </summary>
public interface IParameterFactory
{
    TParameter CreateParameterType<TParameter>() where TParameter : IParameterType;

    Parameter<TParameter> CreateParameter<TParameter>(string displayName, string unit, params Action<TParameter>[] configureCallbacks) where TParameter : IParameterType;
}
