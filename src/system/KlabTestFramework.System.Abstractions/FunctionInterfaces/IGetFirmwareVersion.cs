using System.Threading;
using System.Threading.Tasks;
using Klab.Toolkit.Results;

namespace KlabTestFramework.System.Abstractions.FunctionInterfaces;

/// <summary>
/// Gets the firmware version
/// </summary>
public interface IGetFirmwareVersion
{
    /// <summary>
    /// Gets the firmware version
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Result<string>> GetFirmwareVersionAsync(CancellationToken cancellationToken = default);
}
