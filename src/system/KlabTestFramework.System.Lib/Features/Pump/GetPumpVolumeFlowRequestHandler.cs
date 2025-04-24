using System.Threading;
using System.Threading.Tasks;
using Klab.Toolkit.Event;
using Klab.Toolkit.Results;
using KlabTestFramework.System.Abstractions;
using KlabTestFramework.System.Abstractions.TypeInterfaces;
using UnitsNet;

namespace KlabTestFramework.System.Lib.Features.Pump;

internal sealed class GetPumpVolumeFlowRequestHandler : IRequestHandler<GetPumpVolumeFlowRequest, Result<GetPumpVolumeFlowResponse>>
{
    private readonly ISystemManager _systemManager;

    public GetPumpVolumeFlowRequestHandler(ISystemManager systemManager)
    {
        _systemManager = systemManager;
    }

    public async Task<Result<GetPumpVolumeFlowResponse>> HandleAsync(GetPumpVolumeFlowRequest request, CancellationToken cancellationToken)
    {
        Result<IPump> pump = await _systemManager.GetComponentByIdAsync<IPump>(request.Id, cancellationToken);
        if (pump.IsFailure)
        {
            return Result.Failure<GetPumpVolumeFlowResponse>(pump.Error);
        }

        Result<VolumeFlow> volumeFlow = await pump.Value.GetFlowRateAsync(cancellationToken);
        if (volumeFlow.IsFailure)
        {
            return Result.Failure<GetPumpVolumeFlowResponse>(volumeFlow.Error);
        }

        return Result.Success(new GetPumpVolumeFlowResponse(volumeFlow.Value));
    }
}
