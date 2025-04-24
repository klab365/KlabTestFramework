using System;
using System.Collections.Generic;
using KlabTestFramework.Shared.Parameters;
using KlabTestFramework.Workflow.Abstractions.Features.Validator;
using KlabTestFramework.Workflow.Abstractions.Specifications;
using Microsoft.Extensions.DependencyInjection;

namespace KlabTestFramework.Workflow.Lib;

/// <summary>
/// Represents the configuration for a workflow module.
/// </summary>
public class WorkflowModuleConfiguration
{
    private readonly List<StepType> _stepTypes = new();
    private readonly List<VariableReplaceHandlerType> _variableHandlerTypes = new();
    private readonly List<StepValidatorType> _stepValidatorTypes = new();

    /// <summary>
    /// Gets or sets a value indicating whether to register default steps.
    /// </summary>
    public bool ShouldRegisterDefaultSteps { get; set; } = true;

    /// <summary>
    /// List of step types to register.
    /// </summary>
    public IEnumerable<StepType> StepTypes => _stepTypes;

    /// <summary>
    /// Variable handler types.
    /// </summary>
    public IEnumerable<VariableReplaceHandlerType> VariableHandlerTypes => _variableHandlerTypes;

    /// <summary>
    /// List of step validator types to register.
    /// </summary>
    public IEnumerable<StepValidatorType> StepValidatorTypes => _stepValidatorTypes;

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

    public void AddVariableHandlerType<TParameter, TVariableHandler>() where TParameter : IParameterType where TVariableHandler : IVariableParameterReplaceHandler
    {
        VariableReplaceHandlerType variableHandlerType = new(typeof(TParameter), typeof(TVariableHandler));
        _variableHandlerTypes.Add(variableHandlerType);
    }

    public void AddStepValidatorType<TValidatorHandler>(ServiceLifetime lifetime = ServiceLifetime.Transient) where TValidatorHandler : IStepValidatorHandler
    {
        StepValidatorType stepValidatorType = new(typeof(TValidatorHandler), lifetime);
        _stepValidatorTypes.Add(stepValidatorType);
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

/// <summary>
/// Represents a step validator type in the workflow module configuration.
/// </summary>
/// <param name="ValidatorHandlerType"></param>
/// <param name="Lifetime"></param>
public record StepValidatorType(Type ValidatorHandlerType, ServiceLifetime Lifetime = ServiceLifetime.Transient);
