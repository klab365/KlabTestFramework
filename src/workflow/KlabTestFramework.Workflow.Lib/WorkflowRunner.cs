using System;
using System.Threading.Tasks;
using KlabTestFramework.Workflow.Lib.Contracts;
using KlabTestFramework.Workflow.Lib.Specifications;
using Microsoft.Extensions.Logging;

namespace KlabTestFramework.Workflow.Lib;

/// <summary>
/// Implementation of <see cref="IWorkflowRunner"/>
/// </summary>
public sealed class WorkflowRunner : IWorkflowRunner
{
    private readonly ILogger<WorkflowRunner> _logger;
    private readonly IStepFactory _stepFactory;

    /// <inheritdoc/>
    public event EventHandler<WorkflowStepStatusEventArgs>? StepStatusChanged;

    /// <inheritdoc/>
    public event EventHandler<WorkflowStatusEventArgs>? WorkflowStatusChanged;

    public WorkflowRunner(ILogger<WorkflowRunner> logger, IStepFactory stepFactory)
    {
        _logger = logger;
        _stepFactory = stepFactory;
    }

    public async Task<WorkflowResult> RunAsync(Specifications.Workflow workflow)
    {
        WorkflowStepContext context = new();
        WorkflowStatusChanged?.Invoke(this, new() { Status = WorkflowStatus.Running });
        foreach (StepContainer stepContainer in workflow.Steps)
        {
            try
            {
                StepStatusChanged?.Invoke(this, new() { StepContainer = stepContainer, Status = StepStatus.Running });
                await HandleStep(stepContainer.Step, context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while running workflow {Description}", workflow.Description);
                throw;
            }
            finally
            {
                StepStatusChanged?.Invoke(this, new() { StepContainer = stepContainer, Status = StepStatus.Completed });
            }
        }
        WorkflowStatusChanged?.Invoke(this, new() { Status = WorkflowStatus.Completed });
        return new WorkflowResult { Success = true };
    }

    private async Task HandleStep<TStep>(TStep step, WorkflowStepContext context) where TStep : class, IStep
    {
        try
        {
            StepHandlerWrapperBase stepHandler = _stepFactory.CreateStepHandler(step);
            await stepHandler.HandleAsync(step, context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while handling step {Type}", step.GetType());
            throw;
        }
    }
}
