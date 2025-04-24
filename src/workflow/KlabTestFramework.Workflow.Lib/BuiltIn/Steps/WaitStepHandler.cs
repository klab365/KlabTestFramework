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
        int remainingTimeSec = step.Time.Content.Value;
        while (!cancellationToken.IsCancellationRequested)
        {
            await PublishRemainingTimeAsync(step, remainingTimeSec);
            await _threadProvider.DelayAsync(TimeSpan.FromSeconds(1), cancellationToken);
            remainingTimeSec -= 1;
            if (remainingTimeSec <= 0)
            {
                break;
            }
        }

        await PublishRemainingTimeAsync(step, 0);
        return StepResult.Success(step);
    }

    private async Task PublishRemainingTimeAsync(WaitStep step, int remainingTimeSec)
    {
        Console.WriteLine($"Remaining time: {remainingTimeSec} seconds.");
        await _eventBus.PublishAsync(new StepPublishedInformationEvent(step.Id, $"Remaining time: {remainingTimeSec} seconds."));
    }
}
