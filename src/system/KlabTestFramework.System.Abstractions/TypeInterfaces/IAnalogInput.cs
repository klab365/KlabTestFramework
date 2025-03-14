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

    /// <summary>
    /// Triggers the analog input to take a new measurement.
    /// </summary>
    Task<Result> TriggerAsync(CancellationToken cancellationToken);
}


public static class AnalogInputRequests
{
    /// <summary>
    /// Query request to get the value of an analog input.
    /// </summary>
    public record QueryAnalogInputRequest(string Id) : IRequest<Result<double>>;

    public record QueryAllAnalogInputsRequest(string Alorithm) : IRequest<Result<QueryAllAnalogInputsResponse[]>>;

    public record QueryAllAnalogInputsResponse(string ComponentId, double Value);
}
