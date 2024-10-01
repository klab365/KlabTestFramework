using System.Threading;
using System.Threading.Tasks;
using Klab.Toolkit.Event;
using Klab.Toolkit.Results;
using KlabTestFramework.Workflow.Lib.Specifications;

namespace KlabTestFramework.Workflow.Lib.Features.Runner;


internal class RunSingleStepRequestHandler : IRequestHandler<RunSingleStepRequest, StepResult>
{
    private readonly StepFactory _stepFactory;

    public RunSingleStepRequestHandler(StepFactory stepFactory)
    {
        _stepFactory = stepFactory;
    }

    public async Task<Result<StepResult>> HandleAsync(RunSingleStepRequest request, CancellationToken cancellationToken)
    {
        IStepHandler stepHandler = _stepFactory.CreateStepHandler(request.Step);

        StepResult res = await stepHandler.HandleAsync(request.Step, request.Context, cancellationToken);

        return Result.Success(res); // allways success , because we need the content of stepresults object
    }
}

public record RunSingleStepRequest(IStep Step, WorkflowContext Context) : IRequest<StepResult>;
