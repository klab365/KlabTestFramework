using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Klab.Toolkit.Event;
using Klab.Toolkit.Results;
using KlabTestFramework.System.Abstractions;
using KlabTestFramework.System.Abstractions.Events;
using KlabTestFramework.System.Abstractions.TypeInterfaces;

namespace KlabTestFramework.System.Lib.Features.AnalogIO;

internal sealed class GetAllAnalogInputsRequestHandler : IRequestHandler<AnalogInputRequests.GetAllAnalogInputsRequest, Result<AnalogInputRequests.GetAllAnalogInputsResponse[]>>
{
    private readonly ISystemManager _systemManager;
    private readonly IEventBus _eventBus;
    private readonly object _lock = new();

    public GetAllAnalogInputsRequestHandler(ISystemManager systemManager, IEventBus eventBus)
    {
        _systemManager = systemManager;
        _eventBus = eventBus;
    }

    public async Task<Result<AnalogInputRequests.GetAllAnalogInputsResponse[]>> HandleAsync(AnalogInputRequests.GetAllAnalogInputsRequest request, CancellationToken cancellationToken)
    {
        Result<IEnumerable<IAnalogInput>> analogInputs = await _systemManager.GetAllComponentsOfTypeAsync<IAnalogInput>(cancellationToken);
        if (analogInputs.IsFailure)
        {
            return Result.Failure<AnalogInputRequests.GetAllAnalogInputsResponse[]>(analogInputs.Error);
        }

        // first trigger all analog inputs
        List<TempTriggerResult> triggeredResults = new();
        Parallel.ForEach(analogInputs.Value, async analogInput =>
        {
            string id = analogInput.GetConfig().Id;
            Result resTrigger = await analogInput.TriggerAsync(cancellationToken);
            if (resTrigger.IsFailure)
            {
                await _eventBus.PublishAsync(new SystemComponentErrorEvent(analogInput.GetConfig().Id, resTrigger.Error));
            }

            lock (_lock)
            {
                triggeredResults.Add(new TempTriggerResult(id, resTrigger));
            }
        });

        // get the values in parallel
        List<AnalogInputRequests.GetAllAnalogInputsResponse> responses = new();
        Parallel.ForEach(analogInputs.Value, async analogInput =>
        {
            string id = analogInput.GetConfig().Id;
            if (triggeredResults.Exists(x => x.Id == id && x.Result.IsFailure))
            {
                return;
            }

            Result<double> value = await analogInput.GetValueAsync(cancellationToken);
            if (value.IsSuccess)
            {
                lock (_lock)
                {
                    responses.Add(new AnalogInputRequests.GetAllAnalogInputsResponse(id, value.Value));
                }
            }
        });

        return Result.Success(responses.ToArray());
    }

    /// <summary>
    /// Temp result to store the result of the trigger.
    /// </summary>
    private sealed record TempTriggerResult(string Id, Result Result);
}

