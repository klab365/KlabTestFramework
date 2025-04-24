using System.Threading;
using System.Threading.Tasks;
using Klab.Toolkit.Event;
using Klab.Toolkit.Results;
using KlabTestFramework.System.Abstractions;
using KlabTestFramework.System.Abstractions.TypeInterfaces;

namespace KlabTestFramework.System.Lib.Features.AnalogIO;

internal sealed class GetAnalogInputRequestHandler : IRequestHandler<GetAnalogInputRequest, Result<double>>
{
    private readonly ISystemManager _systemManager;

    public GetAnalogInputRequestHandler(ISystemManager systemManager)
    {
        _systemManager = systemManager;
    }

    public async Task<Result<double>> HandleAsync(GetAnalogInputRequest request, CancellationToken cancellationToken)
    {
        Result<IAnalogInput> analogInput = await _systemManager.GetComponentByIdAsync<IAnalogInput>(request.Id, cancellationToken);
        if (analogInput.IsFailure)
        {
            return Result.Failure<double>(analogInput.Error);
        }

        return await analogInput.Value.GetValueAsync(cancellationToken);
    }
}
