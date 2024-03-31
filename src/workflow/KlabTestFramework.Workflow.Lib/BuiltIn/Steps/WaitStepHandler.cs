using System;
using System.Threading.Tasks;
using Klab.Toolkit.Results;
using KlabTestFramework.Shared.Services;
using KlabTestFramework.Workflow.Lib.Runner;
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
    public async Task<Result> HandleAsync(WaitStep step, IWorkflowContext context)
    {
        TimeSpan remainingTime = step.Time.Content.Value;
        while (!context.CancellationToken.IsCancellationRequested)
        {
            PublishRemainingTime(step, context, remainingTime);
            await _threadProvider.DelayAsync(TimeSpan.FromSeconds(1), context.CancellationToken);
            remainingTime -= TimeSpan.FromSeconds(1);
            if (remainingTime <= TimeSpan.Zero)
            {
                break;
            }
        }

        PublishRemainingTime(step, context, TimeSpan.Zero);
        return Result.Success();
    }

    private static void PublishRemainingTime(WaitStep step, IWorkflowContext context, TimeSpan remainingTime)
    {
        context.PublishMessage(step, $"{remainingTime.TotalSeconds} sec");
    }
}
