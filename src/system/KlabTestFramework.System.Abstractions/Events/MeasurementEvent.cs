using Klab.Toolkit.Event;

namespace KlabTestFramework.System.Abstractions.Events;

/// <summary>
/// Represents a measurement event.
/// </summary>
public record MeasurementEvent(string ComponentId, double Value) : EventBase;

