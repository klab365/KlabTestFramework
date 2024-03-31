using System;
using System.Collections.Generic;
using KlabTestFramework.Shared.Parameters.Types;
using Microsoft.Extensions.DependencyInjection;

namespace KlabTestFramework.Shared.Parameters;

/// <summary>
/// Extends the functionality of the IServiceCollection interface by adding methods for configuring the ParametersModule.
/// </summary>
public static class ParamatersModule
{
    public static IServiceCollection UseParameters(this IServiceCollection services, Action<ParameterModuleConfiguration>? configurationCallback = default)
    {
        ParameterModuleConfiguration configuration = new();
        configurationCallback?.Invoke(configuration);

        services.RegisterParameterTypes(configuration);
        services.AddTransient<ParameterFactory>();

        return services;
    }

    private static void RegisterParameterTypes(this IServiceCollection services, ParameterModuleConfiguration configuration)
    {
        if (configuration.UseDefaultParameters)
        {
            configuration.ParameterTypes.Add(new(nameof(IntParameter), typeof(IntParameter)));
            configuration.ParameterTypes.Add(new(nameof(DoubleParameter), typeof(DoubleParameter)));
            configuration.ParameterTypes.Add(new(nameof(TimeParameter), typeof(TimeParameter)));
            configuration.ParameterTypes.Add(new(nameof(StringParameter), typeof(StringParameter)));
            configuration.ParameterTypes.Add(new(nameof(BoolParameter), typeof(BoolParameter)));
        }

        RegisterSelectableParameterTypes(configuration);
        RegisterListParameterTypes(configuration);
        foreach (ParameterType parameterType in configuration.ParameterTypes)
        {
            services.RegisterParameter(parameterType);
        }
    }

    private static void RegisterSelectableParameterTypes(ParameterModuleConfiguration configuration)
    {
        ParameterType[] parameterTypes = configuration.ParameterTypes.ToArray();
        foreach (ParameterType parameterType in parameterTypes)
        {
            configuration.ParameterTypes.Add(new($"SelectableParameter<{parameterType.Key}>", typeof(SelectableParameter<>).MakeGenericType(parameterType.Parameter)));
        }
    }

    private static void RegisterListParameterTypes(ParameterModuleConfiguration configuration)
    {
        ParameterType[] parameterTypes = configuration.ParameterTypes.ToArray();
        foreach (ParameterType parameterType in parameterTypes)
        {
            configuration.ParameterTypes.Add(new($"ListParameter<{parameterType.Key}>", typeof(ListParameter<>).MakeGenericType(parameterType.Parameter)));
        }
    }

    private static void RegisterParameter(this IServiceCollection services, ParameterType parameterType)
    {
        services.AddTransient(parameterType.Parameter);
        services.AddTransient(provider =>
        {
            ParameterDependencySpecification parameterSpecicifation = new(
                parameterType.Key,
                parameterType.Parameter,
                () => (IParameterType)provider.GetRequiredService(parameterType.Parameter)
            );

            return parameterSpecicifation;
        });
    }
}

public class ParameterModuleConfiguration
{
    public bool UseDefaultParameters { get; set; } = true;

    public List<ParameterType> ParameterTypes { get; } = new();
}

/// <summary>
/// Represents a parameter type in the workflow module configuration.
/// </summary>
/// <param name="Parameter"></param>
/// <returns></returns>
public record ParameterType(string Key, Type Parameter);
