using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Klab.Toolkit.Event;
using KlabTestFramework.Workflow.Abstractions.Specifications;
using KlabTestFramework.Workflow.Lib.Features.Runner;


namespace KlabTestFramework.Workflow.Lib.BuiltIn;

internal class LoopStepHandler : IStepHandler<LoopStep>
{
    private readonly IEventBus _eventBus;

    public LoopStepHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public Task<StepResult> CleanupAsync(LoopStep step, WorkflowContext context, CancellationToken cancellationToken = default)
    {
        throw new System.NotImplementedException();
    }

    public async Task<StepResult> HandleAsync(LoopStep step, WorkflowContext context, CancellationToken cancellationToken = default)
    {
        List<StepResult> resChildren = new();
        for (int i = 0; i < step.IterationCount.Content.Value; i++)
        {
            foreach (IStep child in step.Children)
            {
                StepResult res = await _eventBus.SendAsync(new RunSingleStepRequest(child, context), cancellationToken);
                resChildren.Add(res);
            }
        }

        return StepResult.Collect(step, resChildren.ToArray());
    }

    public Task<StepResult> SetupAsync(LoopStep step, WorkflowContext context, CancellationToken cancellationToken = default)
    {
        throw new System.NotImplementedException();
    }
}
