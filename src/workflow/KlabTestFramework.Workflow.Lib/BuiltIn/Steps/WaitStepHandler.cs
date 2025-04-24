using System;
using System.Threading;
using System.Threading.Tasks;
using Klab.Toolkit.Event;
using KlabTestFramework.Shared.Services;
using KlabTestFramework.Workflow.Abstractions.Events;
using KlabTestFramework.Workflow.Abstractions.Specifications;


namespace KlabTestFramework.Workflow.Lib.BuiltIn;

/// <summary>
/// Handler for the <see cref="WaitStep"/> step.
/// </summary>
public class WaitStepHandler : IStepHandler<WaitStep>
{
    private readonly IThreadProvider _threadProvider;
    private readonly IEventBus _eventBus;

    public WaitStepHandler(IThreadProvider threadProvider, IEventBus eventBus)
    {
        _threadProvider = threadProvider;
        _eventBus = eventBus;
    }

    /// <inheritdoc/>
    public async Task<StepResult> HandleAsync(WaitStep step, WorkflowContext context, CancellationToken cancellationToken = default)
    {
        TimeSpan remainingTime = step.Time.Content.Value;
        while (!cancellationToken.IsCancellationRequested)
        {
            await PublishRemainingTimeAsync(step, remainingTime);
            await _threadProvider.DelayAsync(TimeSpan.FromSeconds(1), cancellationToken);
            remainingTime -= TimeSpan.FromSeconds(1);
            if (remainingTime <= TimeSpan.Zero)
            {
                break;
            }
        }

        await PublishRemainingTimeAsync(step, TimeSpan.Zero);
        return StepResult.Success(step);
    }

    public Task<StepResult> SetupAsync(WaitStep step, WorkflowContext context, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(StepResult.Success(step));
    }

    public Task<StepResult> CleanupAsync(WaitStep step, WorkflowContext context, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(StepResult.Success(step));
    }

    private async Task PublishRemainingTimeAsync(WaitStep step, TimeSpan remainingTime)
    {
        await _eventBus.PublishAsync(new StepPublishedInformationEvent(step.Id, $"Remaining time: {remainingTime}"));
    }
}
