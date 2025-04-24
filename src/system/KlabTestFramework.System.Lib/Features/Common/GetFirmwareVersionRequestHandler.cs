using System.Threading;
using System.Threading.Tasks;
using Klab.Toolkit.Event;
using Klab.Toolkit.Results;
using KlabTestFramework.System.Abstractions;
using KlabTestFramework.System.Abstractions.FunctionInterfaces;

namespace KlabTestFramework.System.Lib.Features.Common;

internal sealed class GetFirmwareVersionRequestHandler : IRequestHandler<GetFirmwareRequests.GetFirmwareVersionRequest, Result<GetFirmwareRequests.GetFirmwareVersionResponse>>
{
    private readonly ISystemManager _systemManager;

    public GetFirmwareVersionRequestHandler(ISystemManager systemManager)
    {
        _systemManager = systemManager;
    }

    public async Task<Result<GetFirmwareRequests.GetFirmwareVersionResponse>> HandleAsync(GetFirmwareRequests.GetFirmwareVersionRequest request, CancellationToken cancellationToken)
    {
        Result<IGetFirmwareVersion> resComponent = await _systemManager.GetComponentByIdAsync<IGetFirmwareVersion>(request.ComponentId, cancellationToken);
        if (resComponent.IsFailure)
        {
            return Result.Failure<GetFirmwareRequests.GetFirmwareVersionResponse>(resComponent.Error);
        }

        Result<string> resReadFirmwareVersion = await resComponent.Value.GetFirmwareVersionAsync(cancellationToken);
        if (resReadFirmwareVersion.IsFailure)
        {
            return Result.Failure<GetFirmwareRequests.GetFirmwareVersionResponse>(resReadFirmwareVersion.Error);
        }

        GetFirmwareRequests.GetFirmwareVersionResponse response = new(request.ComponentId, resReadFirmwareVersion.Value);
        return Result.Success(response);
    }
}
