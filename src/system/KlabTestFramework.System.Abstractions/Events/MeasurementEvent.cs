using System;
using Klab.Toolkit.Event;

namespace KlabTestFramework.System.Abstractions.Events;

/// <summary>
/// Represents a measurement event.
/// </summary>
/// <param name="ComponentId"></param>
/// <param name="Value"></param>
public record MeasurementEvent(string ComponentId, double Value) : IEvent
{
    public Guid Id { get; } = Guid.NewGuid();
}

