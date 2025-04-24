using System;
using Klab.Toolkit.Event;
using Klab.Toolkit.Results;
using UnitsNet;

namespace KlabTestFramework.System.Abstractions.Features;

/// <summary>
/// Request to start a pump.
/// </summary>
/// <param name="Id"></param>
/// <param name="VolumeFlow"></param>
public record PumpOnRequest(string Id, VolumeFlow VolumeFlow) : IRequest<Result>;

/// <summary>
/// Request to stop a pump.
/// </summary>
/// <param name="Id"></param>
public record PumpOffRequest(string Id) : IRequest<Result>;

/// <summary>
/// Request to start a pump for a specific duration.
/// </summary>
/// <param name="Id"></param>
/// <param name="VolumeFlow"></param>
/// <param name="Duration"></param>
public record PumpDurationRequest(string Id, VolumeFlow VolumeFlow, TimeSpan Duration) : IRequest<Result>;

/// <summary>
/// Request to start a pump for a specific volume.
/// </summary>
/// <param name="Id"></param>
/// <param name="VolumeFlow"></param>
/// <param name="Volume"></param>
public record PumpVolumeRequest(string Id, VolumeFlow VolumeFlow, Volume Volume) : IRequest<Result>;

/// <summary>
/// Request to query the volume flow of a pump.
/// </summary>
/// <param name="Id"></param>
public record GetPumpVolumeFlowRequest(string Id) : IRequest<Result<GetPumpVolumeFlowResponse>>;

/// <summary>
/// Response to a query for the volume flow of a pump.
/// </summary>
/// <param name="VolumeFlow"></param>
public record GetPumpVolumeFlowResponse(VolumeFlow VolumeFlow);
