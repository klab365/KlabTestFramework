using Klab.Toolkit.Event;
using Klab.Toolkit.Results;

namespace KlabTestFramework.System.Abstractions.Features;

/// <summary>
/// Query request to get the value of an analog input.
/// </summary>
public record GetAnalogInputRequest(string Id) : IRequest<Result<double>>;

public record GetAllAnalogInputsRequest(string Alorithm) : IRequest<Result<GetAllAnalogInputsResponse[]>>;

public record GetAllAnalogInputsResponse(string ComponentId, double Value);
