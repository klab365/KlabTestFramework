using System.Threading;
using System.Threading.Tasks;
using Klab.Toolkit.Event;
using Klab.Toolkit.Results;
using KlabTestFramework.Workflow.Lib.Ports;
using KlabTestFramework.Workflow.Lib.Specifications;

namespace KlabTestFramework.Workflow.Lib.Features.Editor;

internal sealed class SaveWorkflowRequestHandler : IRequestHandler<SaveWorkflowRequest, Result>
{
    private readonly IWorkflowRepository _repository;

    public SaveWorkflowRequestHandler(IWorkflowRepository workflowRepository)
    {
        _repository = workflowRepository;
    }

    public async Task<Result> HandleAsync(SaveWorkflowRequest request, CancellationToken cancellationToken)
    {
        WorkflowData data = request.Workflow.ToData();
        await _repository.SaveWorkflowAsync(request.FilePath, data, cancellationToken);
        return Result.Success();
    }
}

public record SaveWorkflowRequest(string FilePath, Specifications.Workflow Workflow) : IRequest<Result>;
