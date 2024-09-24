using System;
using System.Threading;
using System.Threading.Tasks;
using Klab.Toolkit.Event;
using Klab.Toolkit.Results;
using UnitsNet;

namespace KlabTestFramework.System.Abstractions.TypeInterfaces;

/// <summary>
/// Represents a pump that can be started and stopped.
/// </summary>
public interface IPump : IComponent
{
    /// <summary>
    /// Sets the flow rate of the pump.
    ///
    /// If the value is not zero, the pump will be started.
    /// If the value is zero, the pump will be stopped.
    /// If the pump is already running, the flow rate will be updated.
    /// </summary>
    /// <param name="volume"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Result> SetFlowRateAsync(VolumeFlow volume, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current volume flow of the pump.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Result<VolumeFlow>> GetFlowRateAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Request to start a pump.
/// </summary>
/// <param name="Id"></param>
/// <param name="VolumeFlow"></param>
public record PumpOnRequest(string Id, VolumeFlow VolumeFlow) : IRequest;

/// <summary>
/// Request to stop a pump.
/// </summary>
/// <param name="Id"></param>
public record PumpOffRequest(string Id) : IRequest;

/// <summary>
/// Request to start a pump for a specific duration.
/// </summary>
/// <param name="Id"></param>
/// <param name="VolumeFlow"></param>
/// <param name="Duration"></param>
public record PumpDurationRequest(string Id, VolumeFlow VolumeFlow, TimeSpan Duration) : IRequest;

/// <summary>
/// Request to start a pump for a specific volume.
/// </summary>
/// <param name="Id"></param>
/// <param name="VolumeFlow"></param>
/// <param name="Volume"></param>
public record PumpVolumeRequest(string Id, VolumeFlow VolumeFlow, Volume Volume) : IRequest;

/// <summary>
/// Request to query the volume flow of a pump.
/// </summary>
/// <param name="Id"></param>
public record QueryPumpVolumeFlowRequest(string Id) : IRequest;

/// <summary>
/// Response to a query for the volume flow of a pump.
/// </summary>
/// <param name="VolumeFlow"></param>
public record QueryPumpVolumeFlowResponse(VolumeFlow VolumeFlow);
