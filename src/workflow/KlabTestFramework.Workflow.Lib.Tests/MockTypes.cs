using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Klab.Toolkit.Results;
using KlabTestFramework.Shared.Parameters;
using KlabTestFramework.Shared.Parameters.Types;
using KlabTestFramework.Workflow.Lib.Features.Runner;
using KlabTestFramework.Workflow.Lib.Specifications;

namespace KlabTestFramework.Workflow.Lib.Tests;

public class MockStep : IStep
{
    public StepId Id { get; set; } = StepId.Empty;

    public Parameter<IntParameter> Counter { get; }

    public MockStep(ParameterFactory parameterFactory)
    {
        Counter = parameterFactory.CreateParameter<IntParameter>
        (
            "Counter",
            "",
            p => p.SetValue(0),
            p => p.AddValidation(v => v >= 0)
        );
    }

    public IEnumerable<IParameter> GetParameters()
    {
        yield return Counter;
    }
}

public class MockStepHandler : IStepHandler<MockStep>
{
    public Task<Result> HandleAsync(MockStep step, WorkflowContext context, CancellationToken cancellationToken = default)
    {
        step.Counter.Content.SetValue(step.Counter.Content.Value + 1);
        return Task.FromResult(Result.Success());
    }
}
