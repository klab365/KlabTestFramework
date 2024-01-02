using System;
using System.Collections.Generic;
using System.Linq;

namespace KlabTestFramework.Workflow.Lib.Specifications;

/// <summary>
/// Implementation of <see cref="IStepFactory"/>
/// </summary>
public class StepFactory : IStepFactory
{
    private readonly IEnumerable<StepSpecification> _stepSpecifications;

    public StepFactory(IEnumerable<StepSpecification> stepSpecifications)
    {
        _stepSpecifications = stepSpecifications;
    }

    /// <inheritdoc/>
    public IStep CreateStep<TStep>() where TStep : IStep
    {
        StepSpecification? stepSpecification = _stepSpecifications.SingleOrDefault(s => s.StepType == typeof(TStep));
        if (stepSpecification == null)
        {
            throw new InvalidOperationException($"Step specification for type {typeof(TStep).Name} not found");
        }

        IStep step = stepSpecification.Factory();
        return step;
    }

    public IStep CreateStep(StepData stepData)
    {
        StepSpecification? stepSpecification = _stepSpecifications.SingleOrDefault(s => s.StepType.Name == stepData.Type);
        if (stepSpecification == null)
        {
            throw new InvalidOperationException($"Step specification for type {stepData.Type} not found");
        }

        IStep step = stepSpecification.Factory();
        return step;
    }
}

public record StepSpecification
{
    /// <summary>
    /// Type of the step
    /// </summary>
    /// <value></value>
    public Type StepType { get; }

    /// <summary>
    /// Factory how to create the step
    /// </summary>
    /// <value></value>
    public Func<IStep> Factory { get; }

    /// <summary>
    /// Create a valid step specification
    /// </summary>
    /// <param name="stepType"></param>
    /// <param name="factory"></param>
    /// <returns></returns>
    public static StepSpecification Create(Type stepType, Func<IStep> factory)
    {
        return new(stepType, factory);
    }

    /// <summary>
    /// Private constructor to force the use of the factory method
    /// </summary>
    /// <param name="key"></param>
    /// <param name="factory"></param>
    private StepSpecification(Type stepType, Func<IStep> factory)
    {
        StepType = stepType;
        Factory = factory;
    }
}
