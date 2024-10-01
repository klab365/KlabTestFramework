using System.Collections.Generic;
using KlabTestFramework.Shared.Parameters;
using KlabTestFramework.Shared.Parameters.Types;
using KlabTestFramework.Workflow.Lib.Specifications;


namespace KlabTestFramework.Workflow.Lib.BuiltIn;

internal class LoopStep : IStep
{
    public List<IStep> Children => throw new System.NotImplementedException();

    public StepId Id { get; set; } = StepId.Empty;

    public Parameter<IntParameter> IterationCount { get; }

    public LoopStep(ParameterFactory parameterFactory)
    {
        IterationCount = parameterFactory.CreateParameter<IntParameter>
        (
            "IterationCount",
            "count",
            p => p.SetValue(1),
            p => p.AddValidation(v => v > 0)
        );
    }

    public IEnumerable<IParameter> GetParameters()
    {
        yield return IterationCount;
    }
}
