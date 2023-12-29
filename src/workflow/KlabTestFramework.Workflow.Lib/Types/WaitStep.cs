using System;
using System.Threading.Tasks;
using Klab.Toolkit.Results;
using KlabTestFramework.Workflow.Lib.Contracts;

namespace KlabTestFramework.Workflow.Lib.Types;

/// <summary>
/// Handler for the <see cref="WaitStep"/> step.
/// </summary>
public class WaitStepHandler : IStepHandler<WaitStep>
{
    /// <inheritdoc/>
    public async Task<Result> HandleAsync(WaitStep step, IWorkflowContext context)
    {
        TimeSpan remainingTime = step.Time;
        Console.WriteLine($"Waiting for {step.Time}");
        PublishRemainingTime(context, remainingTime);
        while (!context.CancellationToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
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


/// <summary>
/// Represents a step that waits for a specified amount of time.
/// </summary>
public class WaitStep : IStep
{
    /// <summary>
    /// Gets or sets the time to wait.
    /// </summary>
    public TimeSpan Time { get; set; } = TimeSpan.Zero;
}

