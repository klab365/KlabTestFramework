using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Klab.Toolkit.Results;
using KlabTestFramework.Workflow.Lib.BuiltIn;
using KlabTestFramework.Workflow.Lib.Runner;
using KlabTestFramework.Workflow.Lib.Specifications;

namespace KlabTestFramework.Workflow.Lib.Tests;

public class MockStep : IStep
{
    public Parameter<IntParameter> Counter { get; }

    public MockStep(IParameterFactory parameterFactory)
    {
        Counter = parameterFactory.CreateParameter<IntParameter>
        (
            "Counter",
            "",
            p => p.SetValue(0),
            p => p.AddValiation(v => v >= 0)
        );
    }

    public IEnumerable<IParameter> GetParameters()
    {
        yield return Counter;
    }
}

public class MockStepHandler : IStepHandler<MockStep>
{
    public Task<Result> HandleAsync(MockStep step, IWorkflowContext context)
    {
        throw new NotImplementedException();
    }
}
