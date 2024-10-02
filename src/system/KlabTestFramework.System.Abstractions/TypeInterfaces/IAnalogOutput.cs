using System.Threading;
using System.Threading.Tasks;
using Klab.Toolkit.Event;
using Klab.Toolkit.Results;

namespace KlabTestFramework.System.Abstractions.TypeInterfaces;

public interface IAnalogOutput : IAnalogInput
{
    /// <summary>
    /// Sets the value of the analog output.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Result> SetValueAsync(double value, CancellationToken cancellationToken = default);
}

/// <summary>
/// Command request to set the value of an analog output.
/// </summary>
/// <param name="Id"></param>
/// <param name="Value"></param>
public record SetAnalogOutputCommandRequest(string Id, double Value) : IRequest<Result>;
