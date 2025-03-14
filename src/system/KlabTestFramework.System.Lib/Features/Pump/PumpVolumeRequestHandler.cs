using System;
using System.Threading;
using System.Threading.Tasks;
using Klab.Toolkit.Event;
using Klab.Toolkit.Results;
using KlabTestFramework.System.Abstractions.TypeInterfaces;

namespace KlabTestFramework.System.Lib.Features.Pump;

internal sealed class PumpVolumeRequestHandler : IRequestHandler<PumpRequests.PumpVolumeRequest, Result>
{
    private readonly IEventBus _eventBus;

    public PumpVolumeRequestHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task<Result> HandleAsync(PumpRequests.PumpVolumeRequest request, CancellationToken cancellationToken)
    {
        // on
        Result resOn = await _eventBus.SendAsync(new PumpRequests.PumpOnRequest(request.Id, request.VolumeFlow), cancellationToken);
        if (resOn.IsFailure)
        {
            return Result.Failure(resOn.Error);
        }

        // wait for volume
        TimeSpan calculatedDuration = request.Volume / request.VolumeFlow;
        await Task.Delay(calculatedDuration, cancellationToken);

        // stop
        Result resOff = await _eventBus.SendAsync(new PumpRequests.PumpOffRequest(request.Id), cancellationToken);
        if (resOff.IsFailure)
        {
            return Result.Failure(resOff.Error);
        }

        return Result.Success();
    }
}
