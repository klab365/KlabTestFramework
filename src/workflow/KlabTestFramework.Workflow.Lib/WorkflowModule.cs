using System;
using Klab.Toolkit.Event;
using KlabTestFramework.Workflow.Lib.BuiltIn;
using KlabTestFramework.Workflow.Lib.BuiltIn.Validator;
using KlabTestFramework.Workflow.Lib.Editor.Adapter;
using KlabTestFramework.Workflow.Lib.Features.Common;
using KlabTestFramework.Workflow.Lib.Features.Editor;
using KlabTestFramework.Workflow.Lib.Features.Runner;
using KlabTestFramework.Workflow.Lib.Features.Validator;
using KlabTestFramework.Workflow.Lib.Ports;
using KlabTestFramework.Workflow.Lib.Specifications;
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
        services.AddWorkflowRepository();
        services.AddWorkflowValidator();
        services.RegisterFeatures();

        return services;
    }

    private static void RegisterFeatures(this IServiceCollection services)
    {
        services.AddRequestResponseHandler<QueryWorkflowRequest, Specifications.Workflow, QueryWorkflowHandler>();
        services.AddRequestResponseHandler<QueryWorkflowRequestByData, Specifications.Workflow, QueryWorkflowHandler>();
        services.AddRequestResponseHandler<CloneWorkflowRequest, Specifications.Workflow, QueryWorkflowHandler>();
        services.AddRequestHandler<SaveWorkflowRequest, SaveWorkflowRequestHandler>();
        services.AddRequestResponseHandler<RunWorkflowRequest, WorkflowResult, RunWorkflowRequestHandler>();
        services.AddRequestResponseHandler<RunSingleStepRequest, StepResult, RunSingleStepRequestHandler>();
        services.AddRequestResponseHandler<ValidateWorkflowRequest, WorkflowValidatorResult, ValidateWorkflowRequestHandler>();
        services.AddRequestHandler<ReplaceWorkflowWithVariablesRequest, ReplaceWorkflowWithVariablesRequesHandler>();
    }

    private static void AddWorkflowValidator(this IServiceCollection services)
    {
        services.AddTransient<IStepValidatorHandler, ParameterValidator>();
    }

    private static void AddWorkflowRepository(this IServiceCollection services)
    {
        services.AddTransient(typeof(IWorkflowRepository), typeof(WorkflowYamlRepository));
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
            configuration.AddStepType<LoopStep, LoopStepHandler>();
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
        services.AddTransient<DefaultVariableParameterReplace>();

        foreach (VariableReplaceHandlerType item in configuration.VariableHandlerTypes)
        {
            services.RegisterVariableReplaceHandler(item.Parameter, item.VariableHandler);
        }
    }

    private static void RegisterVariableReplaceHandler(this IServiceCollection services, Type parameterType, Type variableHandlerType)
    {
        Type genericVariableHandlerType = typeof(IVariableParameterReplaceHandler);
        Type variableType = typeof(Variable<>).MakeGenericType(parameterType);
        services.AddTransient(typeof(IVariableParameterReplaceHandler), variableHandlerType);
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
