using System;
using System.Threading;
using System.Threading.Tasks;

namespace KlabTestFramework.Shared.Services;

public interface IThreadProvider
{
    Task DelayAsync(TimeSpan delay, CancellationToken cancellationToken = default);
}
