using System;
using System.Collections.Generic;

using KlabTestFramework.Workflow.Lib.Editor;
using KlabTestFramework.Workflow.Lib.Editor.Persistence;
using KlabTestFramework.Workflow.Lib.Runner;
using KlabTestFramework.Workflow.Lib.Specifications;

namespace KlabTestFramework.Workflow.Lib;

/// <summary>
/// Represents the configuration for a workflow module.
/// </summary>
public class WorkflowModuleConfiguration
{
    private readonly List<StepType> _stepTypes = new();
    private readonly List<ParameterValueType> _parameterTypes = new();
    private readonly List<VariableHandlerType> _variableHandlerTypes = new();

    /// <summary>
    /// Gets or sets a value indicating whether to register default steps.
    /// </summary>
    public bool ShouldRegisterDefaultSteps { get; set; } = true;

    public bool ShouldRegisterDefaultParameters { get; set; } = true;

    /// <summary>
    /// List of step types to register.
    /// </summary>
    public IEnumerable<StepType> StepTypes => _stepTypes;

    public IEnumerable<ParameterValueType> ParameterTypes => _parameterTypes;

    /// <summary>
    /// Default workflow repository type.
    /// </summary>
    /// <value></value>
    public Func<IWorkflowRepository> DefaultWorkflowRepositoryFactory { get; set; } = () => new WorkflowJsonRepository();

    /// <summary>
    /// Type for the workflow context.
    /// </summary>
    /// <returns></returns>
    public Type WorkflowContextType { get; private set; } = typeof(DefaultWorkflowContext);

    /// <summary>
    /// Variable handler types.
    /// </summary>
    public IEnumerable<VariableHandlerType> VariableHandlerTypes => _variableHandlerTypes;

    /// <summary>
    /// Configure the workflow context type.
    /// </summary>
    /// <typeparam name="TWorkflowContext"></typeparam>
    public void ConfigureWorkflowContext<TWorkflowContext>() where TWorkflowContext : IWorkflowContext
    {
        WorkflowContextType = typeof(TWorkflowContext);
    }

    /// <summary>
    /// Add step type
    /// </summary>
    /// <typeparam name="TStep"></typeparam>
    /// <typeparam name="TStepHandler"></typeparam>
    /// <returns></returns>
    public void AddStepType<TStep, TStepHandler>()
    {
        if (!typeof(TStep).IsAssignableTo(typeof(IStep)))
        {
            throw new ArgumentException($"Type {typeof(TStep).Name} is not assignable to {nameof(IStep)}");
        }

        if (!typeof(TStepHandler).IsAssignableTo(typeof(IStepHandler<>).MakeGenericType(typeof(TStep))))
        {
            throw new ArgumentException($"Type {typeof(TStepHandler).Name} is not assignable to {nameof(IStepHandler<IStep>)}");
        }

        _stepTypes.Add(new(typeof(TStep), typeof(TStepHandler)));
    }

    public void AddParameterType<TParameter>() where TParameter : IParameterType
    {
        if (!typeof(TParameter).IsAssignableTo(typeof(IParameterType)))
        {
            throw new ArgumentException($"Type {typeof(TParameter).Name} is not assignable to {nameof(IParameterType)}");
        }

        _parameterTypes.Add(new(typeof(TParameter)));
    }

    public void AddVariableHandlerType<TParameter, TVariableHandler>() where TParameter : IParameterType where TVariableHandler : IVariableParameterReplaceHandler<TParameter>
    {
        if (!typeof(TParameter).IsAssignableTo(typeof(IParameterType)))
        {
            throw new ArgumentException($"Type {typeof(TParameter).Name} is not assignable to {nameof(IParameterType)}");
        }

        if (!typeof(TVariableHandler).IsAssignableTo(typeof(IVariableParameterReplaceHandler<>).MakeGenericType(typeof(TParameter))))
        {
            throw new ArgumentException($"Type {typeof(TVariableHandler).Name} is not assignable to {nameof(IVariableParameterReplaceHandler<IParameterType>)}");
        }

        VariableHandlerType variableHandlerType = new(typeof(TParameter), typeof(TVariableHandler));
        _variableHandlerTypes.Add(variableHandlerType);
    }
}


/// <summary>
/// Represents a step type in the workflow module configuration.
/// </summary>
/// <param name="Step">The type of the step.</param>
/// <param name="Handler">The type of the handler for the step.</param>
public record StepType(Type Step, Type Handler);

/// <summary>
/// Represents a parameter type in the workflow module configuration.
/// </summary>
/// <param name="Parameter"></param>
/// <returns></returns>
public record ParameterValueType(Type Parameter);

public record VariableHandlerType(Type Parameter, Type VariableHandler);
