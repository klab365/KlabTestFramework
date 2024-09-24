using System.Threading;
using System.Threading.Tasks;
using Klab.Toolkit.Event;
using Klab.Toolkit.Results;
using KlabTestFramework.System.Abstractions;
using KlabTestFramework.System.Abstractions.TypeInterfaces;
using UnitsNet;

namespace KlabTestFramework.System.Lib.Features.Pump;

internal sealed class QueryPumpVolumeFlowRequestHandler : IRequestHandler<QueryPumpVolumeFlowRequest, QueryPumpVolumeFlowResponse>
{
    private readonly ISystemManager _systemManager;

    public QueryPumpVolumeFlowRequestHandler(ISystemManager systemManager)
    {
        _systemManager = systemManager;
    }

    public async Task<Result<QueryPumpVolumeFlowResponse>> HandleAsync(QueryPumpVolumeFlowRequest request, CancellationToken cancellationToken)
    {
        Result<IPump> pump = await _systemManager.GetValidComponentAsync<IPump>(request.Id, cancellationToken);
        if (pump.IsFailure)
        {
            return Result.Failure<QueryPumpVolumeFlowResponse>(pump.Error);
        }

        Result<VolumeFlow> volumeFlow = await pump.Value.GetFlowRateAsync(cancellationToken);
        if (volumeFlow.IsFailure)
        {
            return Result.Failure<QueryPumpVolumeFlowResponse>(volumeFlow.Error);
        }

        return Result.Success(new QueryPumpVolumeFlowResponse(volumeFlow.Value));
    }
}
