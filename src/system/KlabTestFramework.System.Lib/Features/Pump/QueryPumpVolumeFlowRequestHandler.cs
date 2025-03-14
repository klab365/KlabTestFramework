using System.Threading;
using System.Threading.Tasks;
using Klab.Toolkit.Event;
using Klab.Toolkit.Results;
using KlabTestFramework.System.Abstractions;
using KlabTestFramework.System.Abstractions.TypeInterfaces;
using UnitsNet;

namespace KlabTestFramework.System.Lib.Features.Pump;

internal sealed class QueryPumpVolumeFlowRequestHandler : IRequestHandler<PumpRequests.QueryPumpVolumeFlowRequest, Result<PumpRequests.QueryPumpVolumeFlowResponse>>
{
    private readonly ISystemManager _systemManager;

    public QueryPumpVolumeFlowRequestHandler(ISystemManager systemManager)
    {
        _systemManager = systemManager;
    }

    public async Task<Result<PumpRequests.QueryPumpVolumeFlowResponse>> HandleAsync(PumpRequests.QueryPumpVolumeFlowRequest request, CancellationToken cancellationToken)
    {
        Result<IPump> pump = await _systemManager.GetComponentByIdAsync<IPump>(request.Id, cancellationToken);
        if (pump.IsFailure)
        {
            return Result.Failure<PumpRequests.QueryPumpVolumeFlowResponse>(pump.Error);
        }

        Result<VolumeFlow> volumeFlow = await pump.Value.GetFlowRateAsync(cancellationToken);
        if (volumeFlow.IsFailure)
        {
            return Result.Failure<PumpRequests.QueryPumpVolumeFlowResponse>(volumeFlow.Error);
        }

        return Result.Success(new PumpRequests.QueryPumpVolumeFlowResponse(volumeFlow.Value));
    }
}
