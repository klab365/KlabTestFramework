using System.Threading;
using System.Threading.Tasks;
using Klab.Toolkit.Event;
using Klab.Toolkit.Results;
using KlabTestFramework.Shared.Parameters.Types;

namespace KlabTestFramework.System.Abstractions.TypeInterfaces;

public interface IAnalogInput : IComponent
{
    /// <summary>
    /// Gets the gain of the analog input.
    /// </summary>
    DoubleParameter Gain { get; }

    /// <summary>
    /// Gets the offset of the analog input.
    /// </summary>
    DoubleParameter Offset { get; }

    /// <summary>
    /// Gets the unit of the analog input.
    /// </summary>
    StringParameter Unit { get; }

    /// <summary>
    /// Gets the current value of the analog input.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Result<double>> GetValueAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Query request to get the value of an analog input.
/// </summary>
/// <param name="Id"></param>
public record QueryAnalogInputRequest(string Id) : IRequest;
