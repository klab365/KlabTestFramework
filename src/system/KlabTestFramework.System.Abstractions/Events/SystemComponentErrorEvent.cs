using System;
using Klab.Toolkit.Event;
using Klab.Toolkit.Results;

namespace KlabTestFramework.System.Abstractions.Events;

/// <summary>
/// Represents a system component error event.
/// </summary>
public record SystemComponentErrorEvent(string ComponentId, Error Error) : IEvent
{
    public Guid Id { get; } = Guid.NewGuid();

    public DateTimeOffset CreatedAt { get; } = DateTimeOffset.Now;
}

