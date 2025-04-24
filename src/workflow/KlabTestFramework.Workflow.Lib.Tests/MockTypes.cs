using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KlabTestFramework.Shared.Parameters;
using KlabTestFramework.Shared.Parameters.Types;
using KlabTestFramework.Workflow.Abstractions.Features.Validator;
using KlabTestFramework.Workflow.Abstractions.Specifications;
using KlabTestFramework.Workflow.Lib.Specifications;

namespace KlabTestFramework.Workflow.Lib.Tests;

public class MockStep : IStep
{
    public StepId Id { get; set; } = StepId.Empty;

    public StepParameter<IntParameter> Counter { get; }

    public MockStep(ParameterFactory parameterFactory)
    {
        Counter = parameterFactory.CreateParameter<IntParameter>
        (
            "Counter",
            "",
            p => p.SetValue(0)
        );
    }

    public IEnumerable<IStepParameter> GetParameters()
    {
        yield return Counter;
    }

    public Task<WorkflowStepErrorValidation[]> ValidateAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(System.Array.Empty<WorkflowStepErrorValidation>());
    }
}

public class MockStepHandler : IStepHandler<MockStep>
{
    public Task<StepResult> CleanupAsync(MockStep step, WorkflowContext context, CancellationToken cancellationToken = default)
    {
        throw new System.NotImplementedException();
    }

    public Task<StepResult> HandleAsync(MockStep step, WorkflowContext context, CancellationToken cancellationToken = default)
    {
        step.Counter.Content.SetValue(step.Counter.Content.Value + 1);
        return Task.FromResult(StepResult.Success(step));
    }

    public Task<StepResult> SetupAsync(MockStep step, WorkflowContext context, CancellationToken cancellationToken = default)
    {
        throw new System.NotImplementedException();
    }
}
