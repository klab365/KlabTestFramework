using System;
using Klab.Toolkit.Event;

namespace KlabTestFramework.System.Abstractions.Events;

/// <summary>
/// Represents a system initialized event.
/// </summary>
public record SystemInitializedEvent() : IEvent
{
    public Guid Id { get; } = Guid.NewGuid();

    public DateTimeOffset CreatedAt { get; } = DateTimeOffset.Now;
}
