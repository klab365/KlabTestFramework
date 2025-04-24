using System;
using System.Threading;
using System.Threading.Tasks;

namespace KlabTestFramework.System.Abstractions.Common;

/// <summary>
/// A simple lock implementation using SemaphoreSlim.
/// </summary>
public sealed class Lock : IDisposable
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public void Dispose()
    {
        _semaphore.Dispose();
    }

    public async Task LockAsync()
    {
        await _semaphore.WaitAsync();
    }

    public void Release()
    {
        _semaphore.Release();
    }
}
