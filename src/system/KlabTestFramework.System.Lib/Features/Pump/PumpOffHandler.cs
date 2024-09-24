using System.Threading;
using System.Threading.Tasks;
using Klab.Toolkit.Event;
using Klab.Toolkit.Results;
using KlabTestFramework.System.Abstractions;
using KlabTestFramework.System.Abstractions.Events;
using KlabTestFramework.System.Abstractions.TypeInterfaces;
using UnitsNet;

namespace KlabTestFramework.System.Lib.Features.Pump;

internal sealed class PumpOffHandler : IRequestHandler<PumpOffRequest>
{
    private readonly ISystemManager _systemManager;
    private readonly IEventBus _eventBus;

    public PumpOffHandler(ISystemManager systemManager, IEventBus eventBus)
    {
        _systemManager = systemManager;
        _eventBus = eventBus;
    }

    public async Task<Result> HandleAsync(PumpOffRequest request, CancellationToken cancellationToken)
    {
        Result<IPump> pump = await _systemManager.GetValidComponentAsync<IPump>(request.Id, cancellationToken);
        if (pump.IsFailure)
        {
            return Result.Failure(pump.Error);
        }

        VolumeFlow flowRate = VolumeFlow.Zero;
        Result res = await pump.Value.SetFlowRateAsync(flowRate, cancellationToken);
        if (res.IsFailure)
        {
            return Result.Failure(res.Error);
        }

        MeasurementEvent newVolumeFlowEvent = new(request.Id, 0);
        await _eventBus.PublishAsync(newVolumeFlowEvent, cancellationToken);

        return Result.Success();
    }
}
