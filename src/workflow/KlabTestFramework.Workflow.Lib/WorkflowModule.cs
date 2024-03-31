using System;
using KlabTestFramework.Workflow.Lib.BuiltIn;
using KlabTestFramework.Workflow.Lib.Editor;
using KlabTestFramework.Workflow.Lib.Runner;
using KlabTestFramework.Workflow.Lib.Specifications;
using KlabTestFramework.Workflow.Lib.Validator;
using Microsoft.Extensions.DependencyInjection;

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
        configurationCallback?.Invoke(configuration); // apply configuration if provided ;)

        services.AddWorkflowspecification(configuration);
        services.AddWorkflowEditor(configuration);
        services.AddWorkflowRunner(configuration);
        services.AddWorkflowValidator();
        return services;
    }

    private static void AddWorkflowValidator(this IServiceCollection services)
    {
        services.AddTransient<IWorkflowValidator, WorkflowValidator>();
        services.AddTransient<IStepValidatorHandler, ParameterValidator>();
    }

    private static void AddWorkflowEditor(this IServiceCollection services, WorkflowModuleConfiguration configuration)
    {
        services.AddTransient(typeof(IWorkflowRepository), _ => configuration.DefaultWorkflowRepositoryFactory());
        services.AddTransient<IWorkflowEditor, WorkflowEditor>();
    }

    private static void AddWorkflowRunner(this IServiceCollection services, WorkflowModuleConfiguration configuration)
    {
        services.AddTransient(typeof(IWorkflowContext), configuration.WorkflowContextType);
        services.AddTransient<IWorkflowRunner, WorkflowRunner>();
    }

    private static void AddWorkflowspecification(this IServiceCollection services, WorkflowModuleConfiguration configuration)
    {
        services.AddSteps(configuration);
        services.AddVariables(configuration);
    }

    private static void AddSteps(this IServiceCollection services, WorkflowModuleConfiguration configuration)
    {
        services.AddTransient<StepFactory>();
        RegisterSteps(services, configuration);
    }

    private static void RegisterSteps(IServiceCollection services, WorkflowModuleConfiguration configuration)
    {
        if (configuration.ShouldRegisterDefaultSteps)
        {
            configuration.AddStepType<WaitStep, WaitStepHandler>();
            configuration.AddStepType<SubworkflowStep, SubworkflowStepHandler>();
        }

        foreach (StepType stepType in configuration.StepTypes)
        {
            services.RegisterStep(stepType.Step, stepType.Handler);
        }
    }

    private static void RegisterStep(this IServiceCollection services, Type stepType, Type stepHandlerType)
    {
        services.AddTransient(stepType);
        Type genericStepHandlerType = typeof(IStepHandler<>).MakeGenericType(stepType);
        services.AddTransient(genericStepHandlerType, stepHandlerType);
        services.AddTransient(provider =>
        {
            StepSpecification stepSpecification = StepSpecification.Create(
                stepType,
                () => (IStep)provider.GetRequiredService(stepType),
                () => (IStepHandler)provider.GetRequiredService(genericStepHandlerType)
            );
            return stepSpecification;
        });
    }

    private static void AddVariables(this IServiceCollection services, WorkflowModuleConfiguration configuration)
    {
        services.AddTransient<VariableFactory>();
        services.AddTransient(typeof(DefaultVariableParameterReplace<>));
        services.AddTransient<IVariableReplacer, VariableReplacer>();

        foreach (VariableReplaceHandlerType item in configuration.VariableHandlerTypes)
        {
            services.RegisterVariableReplaceHandler(item.Parameter, item.VariableHandler);
        }
    }

    private static void RegisterVariableReplaceHandler(this IServiceCollection services, Type parameterType, Type variableHandlerType)
    {
        Type genericVariableHandlerType = typeof(IVariableParameterReplaceHandler<>).MakeGenericType(parameterType);
        Type variableType = typeof(Variable<>).MakeGenericType(parameterType);
        services.AddTransient(genericVariableHandlerType, variableHandlerType);
        services.AddTransient(variableType);

        services.AddTransient(provider =>
        {
            VariableDependenySpecification specs = new(
                parameterType,
                () => (IVariableParameterReplaceHandler)provider.GetRequiredService(genericVariableHandlerType)
            );

            return specs;
        });
    }
}
