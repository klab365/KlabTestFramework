using System;
using System.Threading;
using System.Threading.Tasks;

namespace KlabTestFramework.Shared.Services;

internal sealed class ThreadProvider : IThreadProvider
{
    public async Task DelayAsync(TimeSpan delay, CancellationToken cancellationToken = default(CancellationToken))
    {
        try
        {
            await Task.Delay(delay, cancellationToken);
        }
        catch (TaskCanceledException)
        {
            // Ignore
        }
    }
}
