using System;
using System.Collections.Generic;

using KlabTestFramework.Workflow.Lib.Editor;
using KlabTestFramework.Workflow.Lib.Editor.Persistence;

namespace KlabTestFramework.Workflow.Lib;

/// <summary>
/// Represents the configuration for a workflow module.
/// </summary>
public class WorkflowModuleConfiguration
{
    private readonly List<StepType> _stepTypes = new();
    private readonly List<ParameterType> _parameterTypes = new();

    /// <summary>
    /// Gets or sets a value indicating whether to register default steps.
    /// </summary>
    public bool ShouldRegisterDefaultSteps { get; set; } = true;

    public bool ShouldRegisterDefaultParameters { get; set; } = true;

    /// <summary>
    /// List of step types to register.
    /// </summary>
    public IEnumerable<StepType> StepTypes => _stepTypes;

    public IEnumerable<ParameterType> ParameterTypes => _parameterTypes;

    /// <summary>
    /// Default workflow repository type.
    /// </summary>
    /// <value></value>
    public Func<IWorkflowRepository> DefaultWorkflowRepositoryFactory { get; set; } = () => new WorkflowJsonRepository();

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
}


/// <summary>
/// Represents a step type in the workflow module configuration.
/// </summary>
/// <param name="Step">The type of the step.</param>
/// <param name="Handler">The type of the handler for the step.</param>
public record StepType(Type Step, Type Handler);

public record ParameterType(Type Parameter);
