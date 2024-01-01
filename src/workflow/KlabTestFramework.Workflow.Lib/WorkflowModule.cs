using System;
using KlabTestFramework.Workflow.Lib.BuildInSteps;

using KlabTestFramework.Workflow.Lib.Editor;
using KlabTestFramework.Workflow.Lib.Runner;
using KlabTestFramework.Workflow.Lib.Specifications;
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

        services.AddWorkflowEditor(configuration);
        services.AddWorkflowRunner();
        services.AddWorkflowspecification(configuration);
        return services;
    }

    private static void AddWorkflowEditor(this IServiceCollection services, WorkflowModuleConfiguration configuration)
    {
        services.AddTransient(typeof(IWorkflowRepository), _ => configuration.DefaultWorkflowRepositoryFactory());
        services.AddTransient<IWorkflowEditor, WorkflowEditor>();
        services.AddTransient<IWorkflowReadEditor, WorkflowEditor>();
    }

    private static void AddWorkflowRunner(this IServiceCollection services)
    {
        services.AddTransient<IWorkflowRunner, WorkflowRunner>();
    }

    private static void AddWorkflowspecification(this IServiceCollection services, WorkflowModuleConfiguration configuration)
    {
        services.AddSteps(configuration);
        services.AddParameters(configuration);
    }

    private static void AddSteps(this IServiceCollection services, WorkflowModuleConfiguration configuration)
    {
        services.AddTransient<IStepFactory, StepFactory>();
        RegisterSteps(services, configuration);
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

    private static void AddParameters(this IServiceCollection services, WorkflowModuleConfiguration configuration)
    {
        services.AddTransient<IParameterFactory, ParameterFactory>();
        services.AddTransient(typeof(Parameter<>));

        if (configuration.ShouldRegisterDefaultParameters)
        {
            configuration.AddParameterType<IntParameter>();
            configuration.AddParameterType<TimeParameter>();
        }

        foreach (ParameterType parameterType in configuration.ParameterTypes)
        {
            services.RegisterParameter(parameterType.Parameter);
        }
    }

    private static void RegisterParameter(this IServiceCollection services, Type parameterType)
    {
        services.AddTransient(parameterType);
        services.AddTransient(provider =>
        {
            ParameterDependencySpecification parameterSpecicifation = new(
                parameterType,
                () => (IParameterType)provider.GetRequiredService(parameterType)
            );

            return parameterSpecicifation;
        });
    }
}
