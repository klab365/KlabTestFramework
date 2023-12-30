using System;
using System.Threading.Tasks;
using Klab.Toolkit.Results;
using KlabTestFramework.Workflow.Lib.Contracts;

namespace KlabTestFramework.Workflow.Lib.Specifications.Steps.StepTypes;

/// <summary>
/// Handler for the <see cref="WaitStep"/> step.
/// </summary>
public class WaitStepHandler : IStepHandler<WaitStep>
{
    /// <inheritdoc/>
    public async Task<Result> HandleAsync(WaitStep step, IWorkflowContext context)
    {
        TimeSpan remainingTime = step.Time.Value;
        Console.WriteLine($"Wait Unit is '{step.SelectedTimeUnit.Value!.Unit}'");
        Console.WriteLine($"Waiting for {step.Time.Value} {step.SelectedTimeUnit.Value!.Unit}");
        PublishRemainingTime(context, remainingTime);
        while (!context.CancellationToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(1), context.CancellationToken);
            remainingTime -= TimeSpan.FromSeconds(1);
            PublishRemainingTime(context, remainingTime);
            if (remainingTime <= TimeSpan.Zero)
            {
                break;
            }
        }

        PublishRemainingTime(context, TimeSpan.Zero);
        return Result.Success();
    }

    private static void PublishRemainingTime(IWorkflowContext context, TimeSpan remainingTime)
    {
        context.PublishMessage($"{remainingTime.TotalSeconds} sec");
    }
}
