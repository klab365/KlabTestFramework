using System;
using Microsoft.Extensions.DependencyInjection;

namespace KlabTestFramework.Workflow.Lib.Specifications;
/// <summary>
/// Implementation of the <see cref="IParameterFactory"/> interface.
/// </summary>
public class ParameterFactory : IParameterFactory
{
    private readonly IServiceProvider _serviceProvider;

    public ParameterFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc/>
    public SingleValueParameter<TValue> CreateSingleValueParameter<TValue>(string displayName, string unit, TValue defaultValue, params Func<TValue, bool>[] isValidCallbacks)
    {
        SingleValueParameter<TValue> parameter = _serviceProvider.GetRequiredService<SingleValueParameter<TValue>>();
        parameter.Init(displayName, unit, defaultValue, isValidCallbacks);
        return parameter;
    }

    /// <inheritdoc/>
    public ChoicesParameter<TValue> CreateChoicesParameter<TValue>(string displayName, string unit, params TValue[] choices)
    {
        ChoicesParameter<TValue> parameter = _serviceProvider.GetRequiredService<ChoicesParameter<TValue>>();
        parameter.Init(displayName, unit, choices);
        return parameter;
    }
}
