using Klab.Toolkit.Event;
using Klab.Toolkit.Results;

namespace KlabTestFramework.System.Abstractions.Events;

/// <summary>
/// Represents a system component error event.
/// </summary>
public record SystemComponentErrorEvent(string ComponentId, Error Error) : EventBase;

