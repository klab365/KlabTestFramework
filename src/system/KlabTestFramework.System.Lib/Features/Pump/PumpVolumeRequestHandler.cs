using System;
using System.Threading;
using System.Threading.Tasks;
using Klab.Toolkit.Event;
using Klab.Toolkit.Results;
using KlabTestFramework.System.Abstractions;
using KlabTestFramework.System.Abstractions.Events;
using KlabTestFramework.System.Abstractions.TypeInterfaces;
using UnitsNet;

namespace KlabTestFramework.System.Lib.Features.Pump;

internal sealed class PumpVolumeRequestHandler : IRequestHandler<PumpVolumeRequest, Result>
{
    private readonly ISystemManager _systemManager;
    private readonly IEventBus _eventBus;

    public PumpVolumeRequestHandler(ISystemManager systemManager, IEventBus eventBus)
    {
        _systemManager = systemManager;
        _eventBus = eventBus;
    }

    public async Task<Result> HandleAsync(PumpVolumeRequest request, CancellationToken cancellationToken)
    {
        Result<IPump> pump = await _systemManager.GetValidComponentAsync<IPump>(request.Id, cancellationToken);
        if (pump.IsFailure)
        {
            return Result.Failure(pump.Error);
        }

        // on
        Result res = await pump.Value.SetFlowRateAsync(request.VolumeFlow, cancellationToken);
        if (res.IsFailure)
        {
            return Result.Failure(res.Error);
        }
        MeasurementEvent newVolumeFlowEvent = new(request.Id, request.VolumeFlow.Value);
        await _eventBus.PublishAsync(newVolumeFlowEvent, cancellationToken);

        // wait for volume
        TimeSpan calculatedDuration = request.Volume / request.VolumeFlow;
        await Task.Delay(calculatedDuration, cancellationToken);

        // stop
        res = await pump.Value.SetFlowRateAsync(VolumeFlow.Zero, cancellationToken);
        if (res.IsFailure)
        {
            return Result.Failure(res.Error);
        }
        newVolumeFlowEvent = new(request.Id, 0);
        await _eventBus.PublishAsync(newVolumeFlowEvent, cancellationToken);

        return Result.Success();
    }
}
