using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KlabTestFramework.Shared.Parameters;
using KlabTestFramework.Shared.Parameters.Types;
using KlabTestFramework.Workflow.Abstractions.Features.Validator;
using KlabTestFramework.Workflow.Abstractions.Specifications;
using KlabTestFramework.Workflow.Lib.Specifications;


namespace KlabTestFramework.Workflow.Lib.BuiltIn;

internal class LoopStep : IStepWithChildren
{
    public List<IStep> Children { get; } = new();

    public StepId Id { get; set; } = StepId.Empty;

    public StepParameter<IntParameter> IterationCount { get; }

    public LoopStep(ParameterFactory parameterFactory)
    {
        IterationCount = parameterFactory.CreateParameter<IntParameter>
        (
            "IterationCount",
            string.Empty,
            p => p.SetValue(1)
        );
    }

    public IEnumerable<IStepParameter> GetParameters()
    {
        yield return IterationCount;
    }

    public Task<WorkflowStepErrorValidation[]> ValidateAsync(CancellationToken cancellationToken = default)
    {
        var errors = new List<WorkflowStepErrorValidation>();
        if (IterationCount.Content.Value < 1)
        {
            errors.Add(new WorkflowStepErrorValidation(this, "Iteration count must be greater than 0."));
        }

        return Task.FromResult(errors.ToArray());
    }
}
