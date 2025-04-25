using System;
using System.Threading;
using System.Threading.Tasks;
using Klab.Toolkit.Results;
using KlabTestFramework.System.Abstractions;

namespace KlabTestFramework.System.Types.Dummy;

public class DummyCommunicator : ICommunicator
{
    public Abstractions.Common.Lock Lock { get; } = new();

    public Task<Result> CloseAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result.Success());
    }

    public Task<Result> OpenAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result.Success());
    }

    public Task<Result<byte[]>> ReadAsync(int responseLength, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result.Success(Array.Empty<byte>()));
    }

    public Task<Result> ResetAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result.Success());
    }

    public Task<Result> WriteAsync(byte[] request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result.Success());
    }
}
