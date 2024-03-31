using System;
using System.Collections.Generic;
using KlabTestFramework.Shared.Parameters;
using KlabTestFramework.Workflow.Lib.Editor;
using KlabTestFramework.Workflow.Lib.Editor.Adapter;
using KlabTestFramework.Workflow.Lib.Runner;
using KlabTestFramework.Workflow.Lib.Specifications;

namespace KlabTestFramework.Workflow.Lib;

/// <summary>
/// Represents the configuration for a workflow module.
/// </summary>
public class WorkflowModuleConfiguration
{
    private readonly List<StepType> _stepTypes = new();
    private readonly List<VariableReplaceHandlerType> _variableHandlerTypes = new();

    /// <summary>
    /// Gets or sets a value indicating whether to register default steps.
    /// </summary>
    public bool ShouldRegisterDefaultSteps { get; set; } = true;

    /// <summary>
    /// List of step types to register.
    /// </summary>
    public IEnumerable<StepType> StepTypes => _stepTypes;

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
    public IEnumerable<VariableReplaceHandlerType> VariableHandlerTypes => _variableHandlerTypes;

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
    public void AddStepType<TStep, TStepHandler>() where TStep : IStep where TStepHandler : IStepHandler<TStep>
    {
        _stepTypes.Add(new(typeof(TStep), typeof(TStepHandler)));
    }

    public void AddVariableHandlerType<TParameter, TVariableHandler>() where TParameter : IParameterType where TVariableHandler : IVariableParameterReplaceHandler<TParameter>
    {
        VariableReplaceHandlerType variableHandlerType = new(typeof(TParameter), typeof(TVariableHandler));
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
/// Represents a variable handler type in the workflow module configuration.
/// </summary>
/// <param name="Parameter"></param>
/// <param name="VariableHandler"></param>
public record VariableReplaceHandlerType(Type Parameter, Type VariableHandler);
