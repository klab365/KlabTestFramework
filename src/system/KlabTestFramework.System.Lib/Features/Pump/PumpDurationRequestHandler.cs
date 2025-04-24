using System.Threading;
using System.Threading.Tasks;
using Klab.Toolkit.Event;
using Klab.Toolkit.Results;
using KlabTestFramework.System.Abstractions.TypeInterfaces;

namespace KlabTestFramework.System.Lib.Features.Pump;

internal sealed class PumpDurationRequestHandler : IRequestHandler<PumpDurationRequest, Result>
{
    private readonly IEventBus _eventBus;

    public PumpDurationRequestHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task<Result> HandleAsync(PumpDurationRequest request, CancellationToken cancellationToken)
    {
        // on
        Result res = await _eventBus.SendAsync(new PumpOnRequest(request.Id, request.VolumeFlow), cancellationToken);
        if (res.IsFailure)
        {
            return Result.Failure(res.Error);
        }

        // wait for duration
        await Task.Delay(request.Duration, cancellationToken);

        // stop
        Result resPumpOff = await _eventBus.SendAsync(new PumpOffRequest(request.Id), cancellationToken);
        if (resPumpOff.IsFailure)
        {
            return Result.Failure(resPumpOff.Error);
        }

        return Result.Success();
    }
}
