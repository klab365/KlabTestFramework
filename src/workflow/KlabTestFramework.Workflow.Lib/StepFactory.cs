using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KlabTestFramework.Workflow.Lib.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace KlabTestFramework.Workflow.Lib;

/// <summary>
/// Implementation of <see cref="IStepFactory"/>
/// </summary>
public class StepFactory : IStepFactory
{
    private static readonly ConcurrentDictionary<Type, StepHandlerWrapperBase> StepHandlers = new();
    private readonly IEnumerable<StepSpecification> _stepSpecifications;

    public StepFactory(IEnumerable<StepSpecification> stepSpecifications)
    {
        _stepSpecifications = stepSpecifications;
    }

    /// <inheritdoc/>
    public IStep CreateStep<TStep>() where TStep : IStep
    {
        StepSpecification stepSpecification = _stepSpecifications.Single(s => s.StepType == typeof(TStep));
        IStep step = stepSpecification.Factory();
        return step;
    }

    /// <inheritdoc/>
    public StepHandlerWrapperBase CreateStepHandler<TStep>(TStep step) where TStep : class, IStep
    {
        StepHandlerWrapperBase stepHandler = StepHandlers.GetOrAdd(step.GetType(), requestType =>
        {
            StepSpecification stepSpecification = _stepSpecifications.Single(s => s.StepType == requestType);
            Type wrapperType = typeof(StepHandlerWrapper<>).MakeGenericType(stepSpecification.StepType);
            object? wrapper = stepSpecification.HandlerFactory();
            if (wrapper == null)
            {
                throw new InvalidOperationException($"Could not create instance of {wrapperType}");
            }

            StepHandlerWrapperBase stepHandler = (StepHandlerWrapperBase)wrapper;
            return stepHandler;
        });

        return stepHandler;
    }
}

/// <summary>
/// This class is used to wrap the step handler in a generic type so that it can be resolved from the DI container.
/// </summary>
public abstract class StepHandlerWrapperBase
{
    /// <summary>
    /// Handles the execution of a step asynchronously.
    /// This method will be resolve the correct step handler from the DI container.
    /// </summary>
    /// <param name="step">The step to be handled.</param>
    /// <param name="context">The context of the workflow step.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public abstract Task HandleAsync(IStep step, WorkflowStepContext context);
}

/// <summary>
/// This class is used to wrap the step handler in a generic type so that it can be resolved from the DI container.
/// </summary>
/// <typeparam name="TStep"></typeparam>
public class StepHandlerWrapper<TStep> : StepHandlerWrapperBase where TStep : class, IStep
{
    private readonly IServiceProvider _serviceProvider;

    public StepHandlerWrapper(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc/>
    public override async Task HandleAsync(IStep step, WorkflowStepContext context)
    {
        IStepHandler<TStep> stepHandler = _serviceProvider.GetRequiredService<IStepHandler<TStep>>();
        await stepHandler.HandleAsync((TStep)step, context);
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
    /// Generate the step handler wrapper to be able resolve the correct handler from the DI container
    /// </summary>
    /// <value></value>
    public Func<StepHandlerWrapperBase> HandlerFactory { get; }

    /// <summary>
    /// Create a valid step specification
    /// </summary>
    /// <param name="stepType"></param>
    /// <param name="factory"></param>
    /// <returns></returns>
    public static StepSpecification Create(Type stepType, Func<IStep> factory, Func<StepHandlerWrapperBase> handlerFactory)
    {
        return new(stepType, factory, handlerFactory);
    }

    /// <summary>
    /// Private constructor to force the use of the factory method
    /// </summary>
    /// <param name="key"></param>
    /// <param name="factory"></param>
    private StepSpecification(Type stepType, Func<IStep> factory, Func<StepHandlerWrapperBase> handlerFactory)
    {
        StepType = stepType;
        Factory = factory;
        HandlerFactory = handlerFactory;
    }
}
