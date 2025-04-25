using System.Threading;
using System.Threading.Tasks;
using Klab.Toolkit.Results;

namespace KlabTestFramework.System.Abstractions;

/// <summary>
/// Interface for a communicator.
/// </summary>
public interface ICommunicator
{
    Common.Lock Lock { get; }

    Task<Result> OpenAsync(CancellationToken cancellationToken = default);

    Task<Result> CloseAsync(CancellationToken cancellationToken = default);

    Task<Result> ResetAsync(CancellationToken cancellationToken = default);

    Task<Result> WriteAsync(byte[] request, CancellationToken cancellationToken = default);

    Task<Result<byte[]>> ReadAsync(int responseLength, CancellationToken cancellationToken = default);
}

public interface ICommunicator<TConfig> : ICommunicator where TConfig : notnull
{
    TConfig Config { get; }

    Task<Result> InitializeAsync(TConfig config, CancellationToken cancellationToken = default);
}
