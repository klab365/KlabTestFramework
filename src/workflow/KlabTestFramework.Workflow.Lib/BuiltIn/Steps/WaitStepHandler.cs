using System;
using System.Threading;
using System.Threading.Tasks;
using KlabTestFramework.Shared.Services;
using KlabTestFramework.Workflow.Lib.Specifications;


namespace KlabTestFramework.Workflow.Lib.BuiltIn;

/// <summary>
/// Handler for the <see cref="WaitStep"/> step.
/// </summary>
public class WaitStepHandler : IStepHandler<WaitStep>
{
    private readonly IThreadProvider _threadProvider;

    public WaitStepHandler(IThreadProvider threadProvider)
    {
        _threadProvider = threadProvider;
    }

    /// <inheritdoc/>
    public async Task<StepResult> HandleAsync(WaitStep step, WorkflowContext context, CancellationToken cancellationToken = default)
    {
        TimeSpan remainingTime = step.Time.Content.Value;
        while (!cancellationToken.IsCancellationRequested)
        {
            PublishRemainingTime(step, remainingTime);
            await _threadProvider.DelayAsync(TimeSpan.FromSeconds(1), cancellationToken);
            remainingTime -= TimeSpan.FromSeconds(1);
            if (remainingTime <= TimeSpan.Zero)
            {
                break;
            }
        }

        PublishRemainingTime(step, TimeSpan.Zero);
        return StepResult.Success(step);
    }

    private static void PublishRemainingTime(WaitStep step, TimeSpan remainingTime)
    {
        Console.WriteLine($"Remaining time for step {step.Id}: {remainingTime}");
    }
}
