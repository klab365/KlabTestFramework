using System;
using KlabTestFramework.Workflow.Lib.Contracts;
using KlabTestFramework.Workflow.Lib.Runner;
using KlabTestFramework.Workflow.Lib.Specifications;
using KlabTestFramework.Workflow.Lib.Specifications.Parameters;
using KlabTestFramework.Workflow.Lib.Specifications.Parameters.ParameterTypes;
using KlabTestFramework.Workflow.Lib.Specifications.Steps;
using KlabTestFramework.Workflow.Lib.Specifications.Steps.StepTypes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace KlabTestFramework.Workflow.Lib;

/// <summary>
/// Extends the functionality of the IServiceCollection interface by adding methods for configuring the WorkflowLib module.
/// </summary>
/// <param name="services">The IServiceCollection instance.</param>
/// <param name="configurationCallback">An optional callback to configure the WorkflowModuleConfiguration.</param>
/// <returns>The modified IServiceCollection instance.</returns>
public static class WorkflowModule
{
    /// <summary>
    /// Extends the functionality of the IServiceCollection interface by adding methods for configuring the WorkflowLib module.
    /// </summary>
    /// <param name="services">The IServiceCollection instance.</param>
    /// <param name="configurationCallback">An optional callback to configure the WorkflowModuleConfiguration.</param>
    /// <returns>The modified IServiceCollection instance.</returns>
    public static IServiceCollection UseWorkflowLib(this IServiceCollection services, Action<WorkflowModuleConfiguration>? configurationCallback = default)
    {
        WorkflowModuleConfiguration configuration = new();
        configurationCallback?.Invoke(configuration);

        services.AddSingleton<IWorkflowFactory, WorkflowFactoy>();
        services.AddTransient<Specifications.Workflow>();
        services.AddSingleton<IWorkflowRunner, WorkflowRunner>();
        services.AddSteps(configuration);
        services.AddParameters(configuration);
        return services;
    }

    private static IServiceCollection AddSteps(this IServiceCollection services, WorkflowModuleConfiguration configuration)
    {
        services.AddSingleton<IStepFactory, StepFactory>();
        RegisterSteps(services, configuration);
        return services;
    }

    private static IServiceCollection AddParameters(this IServiceCollection services, WorkflowModuleConfiguration configuration)
    {
        services.AddTransient<IParameterFactory, ParameterFactory>();
        services.AddTransient(typeof(SingleValueParameter<>));
        services.AddTransient(typeof(ChoicesParameter<>));
        return services;
    }

    private static void RegisterSteps(IServiceCollection services, WorkflowModuleConfiguration configuration)
    {
        if (configuration.ShouldRegisterDefaultSteps)
        {
            configuration.AddStepType<WaitStep, WaitStepHandler>();
        }

        foreach (StepType stepType in configuration.StepTypes)
        {
            services.RegisterStep(stepType.Step, stepType.Handler);
        }
    }

    private static void RegisterStep(this IServiceCollection services, Type stepType, Type stepHandlerType)
    {
        services.TryAddTransient(stepType);
        Type genericStepHandlerType = typeof(IStepHandler<>).MakeGenericType(stepType);
        services.TryAddTransient(genericStepHandlerType, stepHandlerType);
        Type genericStepHandlerWrapperType = typeof(StepHandlerWrapper<>).MakeGenericType(stepType);
        services.TryAddTransient(genericStepHandlerWrapperType);
        services.TryAddTransient(provider =>
        {
            StepSpecification stepSpecification = StepSpecification.Create(
                stepType,
                () => (IStep)provider.GetRequiredService(stepType),
                () => (StepHandlerWrapperBase)provider.GetRequiredService(genericStepHandlerWrapperType)
            );
            return stepSpecification;
        });
    }
}
