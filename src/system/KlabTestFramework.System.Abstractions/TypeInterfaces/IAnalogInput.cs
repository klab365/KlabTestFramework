using System.Threading;
using System.Threading.Tasks;
using Klab.Toolkit.Event;
using Klab.Toolkit.Results;
using KlabTestFramework.Shared.Parameters.Types;

namespace KlabTestFramework.System.Abstractions.TypeInterfaces;

/// <summary>
/// Interface for analog input components.
/// </summary>
public interface IAnalogInput : IComponent
{
    DoubleParameter Gain { get; }

    DoubleParameter Offset { get; }

    StringParameter Unit { get; }

    Task<Result<double>> GetValueAsync(CancellationToken cancellationToken = default);

    Task<Result> TriggerAsync(CancellationToken cancellationToken);
}


/// <summary>
/// Query request to get the value of an analog input.
/// </summary>
public record GetAnalogInputRequest(string Id) : IRequest<Result<double>>;

public record GetAllAnalogInputsRequest(string Alorithm) : IRequest<Result<GetAllAnalogInputsResponse[]>>;

public record GetAllAnalogInputsResponse(string ComponentId, double Value);
