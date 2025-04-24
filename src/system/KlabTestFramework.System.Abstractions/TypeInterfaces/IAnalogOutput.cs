using System.Threading;
using System.Threading.Tasks;
using Klab.Toolkit.Results;

namespace KlabTestFramework.System.Abstractions.TypeInterfaces;

/// <summary>
/// Interface for analog output components.
/// </summary>
public interface IAnalogOutput : IAnalogInput
{
    Task<Result> SetValueAsync(double value, CancellationToken cancellationToken = default);
}


