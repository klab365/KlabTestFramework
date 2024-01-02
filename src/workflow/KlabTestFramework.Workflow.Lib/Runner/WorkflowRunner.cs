using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Klab.Toolkit.Results;
using KlabTestFramework.Workflow.Lib.Runner;
using KlabTestFramework.Workflow.Lib.Specifications;
using KlabTestFramework.Workflow.Lib.Validator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace KlabTestFramework.Workflow.Lib;

/// <summary>
/// Implementation of <see cref="IWorkflowRunner"/>
/// </summary>
public class WorkflowRunner : IWorkflowRunner
{
    private readonly ILogger<WorkflowRunner> _logger;
    private readonly Func<IWorkflowContext> _workflowContextFactory;
    private readonly IWorkflowValidator _validator;
    private readonly IVariableReplacer _variableReplacer;
    private readonly Func<Type, StepHandlerWrapperBase> _stepHandlerWrapperFactory;
    private readonly ConcurrentDictionary<Type, StepHandlerWrapperBase> _stepHandlers = new();

    /// <inheritdoc/>
    public event EventHandler<WorkflowStepStatusEventArgs>? StepStatusChanged;

    /// <inheritdoc/>
    public event EventHandler<WorkflowStatusEventArgs>? WorkflowStatusChanged;

    public WorkflowRunner(
        IWorkflowValidator validator,
        IVariableReplacer variableReplacer,
        Func<Type, StepHandlerWrapperBase> stepHandlerWrapperFactory,
        ILogger<WorkflowRunner> logger,
        Func<IWorkflowContext> workflowContextFactory)
    {
        _logger = logger;
        _workflowContextFactory = workflowContextFactory;
        _validator = validator;
        _variableReplacer = variableReplacer;
        _stepHandlerWrapperFactory = stepHandlerWrapperFactory;
    }

    /// <inheritdoc/>
    public async Task<WorkflowResult> RunAsync(Specifications.Workflow workflow)
    {
        IWorkflowContext context = _workflowContextFactory();
        context.Variables = workflow.Variables.ToList();

        await ReplaceVariableValuesToParametersAsync(workflow);

        Result resCheckErrors = await CheckWorkflowHasErrorsAsync(workflow);
        if (resCheckErrors.IsFailure)
        {
            return new(false);
        }

        return await HandleWorkflowAsync(workflow, context);
    }

    private async Task ReplaceVariableValuesToParametersAsync(Specifications.Workflow workflow)
    {
        await _variableReplacer.ReplaceVariablesWithTheParametersAsync(workflow);
    }

    private async Task<WorkflowResult> HandleWorkflowAsync(Specifications.Workflow workflow, IWorkflowContext context)
    {
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
        return new(true);
    }

    private async Task<Result> CheckWorkflowHasErrorsAsync(Specifications.Workflow workflow)
    {
        WorkflowValidatorResult res = await _validator.ValidateAsync(workflow);
        if (res.Errors.Count != 0)
        {
            return WorkflowRunnerErrors.WorkflowHasErrors;
        }

        return Result.Success();
    }

    /// <summary>
    /// Handle the step handler
    /// For this we will use the <see cref="StepHandlerWrapperBase"/> to get the correct step handler from the DI container
    /// </summary>
    /// <typeparam name="TStep"></typeparam>
    private async Task<Result> HandleStep<TStep>(TStep step, IWorkflowContext context) where TStep : class, IStep
    {
        try
        {
            StepHandlerWrapperBase stepHandler = _stepHandlers.GetOrAdd(step.GetType(), requestType =>
            {
                StepHandlerWrapperBase wrapper = _stepHandlerWrapperFactory(requestType);
                return wrapper;
            });

            return await stepHandler.HandleAsync(step, context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while handling step {Type}", step.GetType());
            throw;
        }
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
    public abstract Task<Result> HandleAsync(IStep step, IWorkflowContext context);
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
    public override async Task<Result> HandleAsync(IStep step, IWorkflowContext context)
    {
        IStepHandler<TStep> stepHandler = _serviceProvider.GetRequiredService<IStepHandler<TStep>>();
        return await stepHandler.HandleAsync((TStep)step, context);
    }
}
