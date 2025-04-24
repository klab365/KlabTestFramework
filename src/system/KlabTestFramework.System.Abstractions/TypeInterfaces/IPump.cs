using System.Threading;
using System.Threading.Tasks;
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
