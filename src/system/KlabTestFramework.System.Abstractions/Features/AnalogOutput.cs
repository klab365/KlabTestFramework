using Klab.Toolkit.Event;
using Klab.Toolkit.Results;

namespace KlabTestFramework.System.Abstractions.Features;

/// <summary>
/// Command request to set the value of an analog output.
/// </summary>
public record SetAnalogOutputCommandRequest(string Id, double Value) : IRequest<Result>;
