using System.Threading;
using System.Threading.Tasks;
using Klab.Toolkit.Event;
using Klab.Toolkit.Results;

namespace KlabTestFramework.System.Abstractions.TypeInterfaces;

/// <summary>
/// Interface for analog output components.
/// </summary>
public interface IAnalogOutput : IAnalogInput
{
    Task<Result> SetValueAsync(double value, CancellationToken cancellationToken = default);
}

/// <summary>
/// Requests for analog output components.
/// </summary>
public static class AnalogOutputRequests
{

}

/// <summary>
/// Command request to set the value of an analog output.
/// </summary>
public record SetAnalogOutputCommandRequest(string Id, double Value) : IRequest<Result>;
