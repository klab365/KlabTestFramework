using System;
using System.Linq;
using Klab.Toolkit.Results;

namespace KlabTestFramework.Workflow.Abstractions.Specifications;

/// <summary>
/// Object representing the result of a step.
/// </summary>
public record StepResult
{
    public IStep Step { get; }
    public StepResult[] Children { get; }
    public Error Error { get; }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public static StepResult Success(IStep step, params StepResult[] children)
    {
        return new StepResult(step, true, Error.None(), children);
    }

    public static StepResult Failure(IStep step, Error error, params StepResult[] children)
    {
        return new StepResult(step, false, error, children);
    }

    public static StepResult Collect(IStep step, StepResult[] stepResults)
    {
        if (Array.Exists(stepResults, r => r.IsFailure))
        {
            return Failure(step, Error.Create("", ""), stepResults.ToArray());
        }
        else
        {
            return Success(step, stepResults.ToArray());
        }
    }

    private StepResult(IStep step, bool isSuccess, Error error, StepResult[] children)
    {
        Step = step;
        IsSuccess = isSuccess;
        Error = error;
        Children = children;
    }
}
