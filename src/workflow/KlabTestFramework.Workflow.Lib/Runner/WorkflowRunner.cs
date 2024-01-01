using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using KlabTestFramework.Workflow.Lib.Editor;
using KlabTestFramework.Workflow.Lib.Runner;
using KlabTestFramework.Workflow.Lib.Specifications;
using Microsoft.Extensions.Logging;

namespace KlabTestFramework.Workflow.Lib;

/// <summary>
/// Implementation of <see cref="IWorkflowRunner"/>
/// </summary>
public sealed class WorkflowRunner : IWorkflowRunner
{
    private readonly ILogger<WorkflowRunner> _logger;
    private readonly IEnumerable<StepSpecification> _stepSpecifications;
    private static readonly ConcurrentDictionary<Type, StepHandlerWrapperBase> StepHandlers = new();

    /// <inheritdoc/>
    public event EventHandler<WorkflowStepStatusEventArgs>? StepStatusChanged;

    /// <inheritdoc/>
    public event EventHandler<WorkflowStatusEventArgs>? WorkflowStatusChanged;

    public WorkflowRunner(ILogger<WorkflowRunner> logger, IEnumerable<StepSpecification> stepSpecifications)
    {
        _logger = logger;
        _stepSpecifications = stepSpecifications;
    }

    public async Task<WorkflowResult> RunAsync(Specifications.Workflow workflow)
    {
        WorkflowStepContext context = new();
        WorkflowStatusChanged?.Invoke(this, new() { Status = WorkflowStatus.Running });
        foreach (StepContainer stepContainer in workflow.Steps)
        {
            try
            {
                StepStatusChanged?.Invoke(this, new() { StepId = stepContainer.Id, Status = StepStatus.Running });
                await HandleStep(stepContainer.Step, context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while running workflow");
                throw;
            }
            finally
            {
                StepStatusChanged?.Invoke(this, new() { StepId = stepContainer.Id, Status = StepStatus.Completed });
            }
        }
        WorkflowStatusChanged?.Invoke(this, new() { Status = WorkflowStatus.Completed });
        return new WorkflowResult { Success = true };
    }

    /// <summary>
    /// Handle the step handler
    /// For this we will use the <see cref="StepHandlerWrapperBase"/> to get the correct step handler from the DI container
    /// </summary>
    /// <typeparam name="TStep"></typeparam>
    private async Task HandleStep<TStep>(TStep step, WorkflowStepContext context) where TStep : class, IStep
    {
        try
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

            await stepHandler.HandleAsync(step, context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while handling step {Type}", step.GetType());
            throw;
        }
    }
}
