using System;
using System.Threading.Tasks;
using Klab.Toolkit.Results;
using KlabTestFramework.Workflow.Lib.Runner;
using KlabTestFramework.Workflow.Lib.Specifications;


namespace KlabTestFramework.Workflow.Lib.BuiltIn;

/// <summary>
/// Handler for the <see cref="WaitStep"/> step.
/// </summary>
public class WaitStepHandler : IStepHandler<WaitStep>
{
    /// <inheritdoc/>
    public async Task<Result> HandleAsync(WaitStep step, IWorkflowContext context)
    {
        TimeSpan remainingTime = step.Time.Content.Value;
        PublishRemainingTime(step, context, remainingTime);
        while (!context.CancellationToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(1), context.CancellationToken);
            remainingTime -= TimeSpan.FromSeconds(1);
            PublishRemainingTime(step, context, remainingTime);
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
