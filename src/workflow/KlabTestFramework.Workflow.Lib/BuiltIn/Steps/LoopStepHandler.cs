using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Klab.Toolkit.Event;
using KlabTestFramework.Workflow.Lib.Features.Runner;
using KlabTestFramework.Workflow.Lib.Specifications;


namespace KlabTestFramework.Workflow.Lib.BuiltIn;

internal class LoopStepHandler : IStepHandler<LoopStep>
{
    private readonly IEventBus _eventBus;

    public LoopStepHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
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
}
