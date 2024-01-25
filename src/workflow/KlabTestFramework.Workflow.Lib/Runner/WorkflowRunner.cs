using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Klab.Toolkit.Results;
using KlabTestFramework.Workflow.Lib.Runner;
using KlabTestFramework.Workflow.Lib.Specifications;
using KlabTestFramework.Workflow.Lib.Validator;
using Microsoft.Extensions.Logging;

namespace KlabTestFramework.Workflow.Lib;

/// <summary>
/// Implementation of <see cref="IWorkflowRunner"/>
/// </summary>
public class WorkflowRunner : IWorkflowRunner
{
    private readonly ILogger<WorkflowRunner> _logger;
    private readonly IWorkflowValidator _validator;
    private readonly IVariableReplacer _variableReplacer;
    private readonly IStepFactory _stepFactory;

    /// <inheritdoc/>
    public event EventHandler<WorkflowStepStatusEventArgs>? StepStatusChanged;

    /// <inheritdoc/>
    public event EventHandler<WorkflowStatusEventArgs>? WorkflowStatusChanged;

    public WorkflowRunner(
        IWorkflowValidator validator,
        IVariableReplacer variableReplacer,
        IStepFactory stepFactory,
        ILogger<WorkflowRunner> logger)
    {
        _logger = logger;
        _validator = validator;
        _variableReplacer = variableReplacer;
        _stepFactory = stepFactory;
    }

    /// <inheritdoc/>
    public async Task<WorkflowResult> RunAsync(Specifications.Workflow workflow, IWorkflowContext context)
    {
        context.Variables = workflow.Variables.ToArray();
        await ReplaceVariableValuesToParametersAsync(workflow);

        Result resCheckErrors = await CheckWorkflowHasErrorsAsync(workflow);
        if (resCheckErrors.IsFailure)
        {
            return new(false);
        }

        return await HandleWorkflowAsync(workflow, context);
    }

    public async Task<WorkflowResult> RunSubworkflowAsync(ISubworkflowStep subworkflowStep, IWorkflowContext context)
    {
        return await HandleStepsAsync(subworkflowStep.Children, context);
    }

    private async Task ReplaceVariableValuesToParametersAsync(Specifications.Workflow workflow)
    {
        await _variableReplacer.ReplaceVariablesWithTheParametersAsync(workflow);
    }

    private async Task<WorkflowResult> HandleWorkflowAsync(Specifications.Workflow workflow, IWorkflowContext context)
    {
        WorkflowStatusChanged?.Invoke(this, new() { Status = WorkflowStatus.Running });

        WorkflowResult res = await HandleStepsAsync(workflow.Steps, context);

        WorkflowStatusChanged?.Invoke(this, new() { Status = WorkflowStatus.Completed });
        return res;
    }

    private async Task<WorkflowResult> HandleStepsAsync(IEnumerable<IStep> steps, IWorkflowContext context)
    {
        foreach (IStep step in steps)
        {
            try
            {
                StepStatusChanged?.Invoke(this, new() { StepId = step.Id, Status = StepStatus.Running });
                await HandleStep(step, context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while running workflow");
                throw;
            }
            finally
            {
                StepStatusChanged?.Invoke(this, new() { StepId = step.Id, Status = StepStatus.Completed });
            }
        }

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
            IStepHandler stepHandler = _stepFactory.CreateStepHandler(step);
            return await stepHandler.HandleAsync(step, context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while handling step {Type}", step.GetType());
            throw;
        }
    }
}
