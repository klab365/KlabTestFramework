using System.Threading;
using System.Threading.Tasks;
using Klab.Toolkit.Event;
using Klab.Toolkit.Results;
using KlabTestFramework.System.Abstractions;
using KlabTestFramework.System.Abstractions.TypeInterfaces;

namespace KlabTestFramework.System.Lib.Features.AnalogIO;

internal sealed class QueryAnalogInputRequestHandler : IRequestHandler<QueryAnalogInputRequest, double>
{
    private readonly ISystemManager _systemManager;

    public QueryAnalogInputRequestHandler(ISystemManager systemManager)
    {
        _systemManager = systemManager;
    }

    public async Task<Result<double>> HandleAsync(QueryAnalogInputRequest request, CancellationToken cancellationToken)
    {
        Result<IAnalogInput> analogInput = await _systemManager.GetValidComponentAsync<IAnalogInput>(request.Id, cancellationToken);
        if (analogInput.IsFailure)
        {
            return Result.Failure<double>(analogInput.Error);
        }

        return await analogInput.Value.GetValueAsync(cancellationToken);
    }
}
