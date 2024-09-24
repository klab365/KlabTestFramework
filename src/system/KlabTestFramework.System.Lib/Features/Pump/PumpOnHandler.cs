using System.Threading;
using System.Threading.Tasks;
using Klab.Toolkit.Event;
using Klab.Toolkit.Results;
using KlabTestFramework.System.Abstractions;
using KlabTestFramework.System.Abstractions.Events;
using KlabTestFramework.System.Abstractions.TypeInterfaces;

namespace KlabTestFramework.System.Lib.Features.Pump;

/// <summary>
/// Handler for the <see cref="PumpOnRequest"/>.
/// </summary>
internal sealed class PumpOnHandler : IRequestHandler<PumpOnRequest>
{
    private readonly ISystemManager _systemManager;
    private readonly IEventBus _eventBus;

    public PumpOnHandler(ISystemManager systemManager, IEventBus eventBus)
    {
        _systemManager = systemManager;
        _eventBus = eventBus;
    }

    public async Task<Result> HandleAsync(PumpOnRequest request, CancellationToken cancellationToken)
    {
        Result<IPump> pump = await _systemManager.GetValidComponentAsync<IPump>(request.Id, cancellationToken);
        if (pump.IsFailure)
        {
            return Result.Failure(pump.Error);
        }

        Result res = await pump.Value.SetFlowRateAsync(request.VolumeFlow, cancellationToken);
        if (res.IsFailure)
        {
            return Result.Failure(res.Error);
        }

        MeasurementEvent newVolumeFlowEvent = new(request.Id, request.VolumeFlow.Value);
        await _eventBus.PublishAsync(newVolumeFlowEvent, cancellationToken);

        return Result.Success();
    }
}
