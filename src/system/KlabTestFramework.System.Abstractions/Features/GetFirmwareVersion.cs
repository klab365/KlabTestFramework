using Klab.Toolkit.Event;
using Klab.Toolkit.Results;

namespace KlabTestFramework.System.Abstractions.Features.Common;

/// <summary>
/// Request to get the firmware version of a component
/// </summary>
public record GetFirmwareVersionRequest(string ComponentId) : IRequest<Result<GetFirmwareVersionResponse>>;

/// <summary>
/// Response to get the firmware version of a component
/// </summary>
public record GetFirmwareVersionResponse(string ComponentId, string FirmwareVersion);

/// <summary>
/// Request to get all firmware versions
/// </summary>
public record GetAllFirmwareVersionsRequest : IRequest<Result<GetAllFirmwareVersionsResponse>>;

/// <summary>
/// Response to get all firmware versions
/// </summary>
public record GetAllFirmwareVersionsResponse(GetFirmwareVersionResponse[] FirmwareVersions);
