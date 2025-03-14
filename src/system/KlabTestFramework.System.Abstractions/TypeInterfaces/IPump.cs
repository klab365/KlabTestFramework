using System;
using System.Threading;
using System.Threading.Tasks;
using Klab.Toolkit.Event;
using Klab.Toolkit.Results;
using UnitsNet;

namespace KlabTestFramework.System.Abstractions.TypeInterfaces;

/// <summary>
/// Interface for pump components.
/// </summary>
public interface IPump : IComponent
{
    Task<Result> SetFlowRateAsync(VolumeFlow volume, CancellationToken cancellationToken = default);

    Task<Result<VolumeFlow>> GetFlowRateAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Requests for pump components.
/// </summary>
public static class PumpRequests
{
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
    public record QueryPumpVolumeFlowRequest(string Id) : IRequest<Result<QueryPumpVolumeFlowResponse>>;

    /// <summary>
    /// Response to a query for the volume flow of a pump.
    /// </summary>
    /// <param name="VolumeFlow"></param>
    public record QueryPumpVolumeFlowResponse(VolumeFlow VolumeFlow);
}
