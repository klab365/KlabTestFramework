using System.Threading;
using System.Threading.Tasks;
using Klab.Toolkit.Event;
using Klab.Toolkit.Results;
using KlabTestFramework.System.Abstractions;
using KlabTestFramework.System.Abstractions.TypeInterfaces;

namespace KlabTestFramework.System.Lib.Features.AnalogIO;

internal sealed class SetAnalogOutputCommandRequestHandler : IRequestHandler<SetAnalogOutputCommandRequest, Result>
{
    private readonly ISystemManager _systemManager;

    public SetAnalogOutputCommandRequestHandler(ISystemManager systemManager)
    {
        _systemManager = systemManager;
    }

    public async Task<Result> HandleAsync(SetAnalogOutputCommandRequest request, CancellationToken cancellationToken)
    {
        Result<IAnalogOutput> analogOutput = await _systemManager.GetValidComponentAsync<IAnalogOutput>(request.Id, cancellationToken);
        if (analogOutput.IsFailure)
        {
            return Result.Failure(analogOutput.Error);
        }

        return await analogOutput.Value.SetValueAsync(request.Value, cancellationToken);
    }
}
