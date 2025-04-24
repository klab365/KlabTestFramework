using Klab.Toolkit.Event;
using KlabTestFramework.Workflow.Abstractions.Specifications;

namespace KlabTestFramework.Workflow.Abstractions.Events;

public record StepPublishedInformationEvent(StepId StepId, string Message) : EventBase;
