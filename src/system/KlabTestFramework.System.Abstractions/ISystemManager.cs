using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Klab.Toolkit.Results;

namespace KlabTestFramework.System.Abstractions;

public interface ISystemManager
{
    IEnumerable<IComponent> Components { get; }

    Task<Result> InitializeAsync(string path, CancellationToken cancellationToken = default);
}
