using Klab.Toolkit.Results;

namespace KlabTestFramework.Workflow.Abstractions;

public static class WorkflowModuleErrors
{
    private const string Code = "Workflow";

    public static Error WorkflowHasErrors => Error.Create(Code, "Workflow has errors");

    public static Error ErrorWhileHandlingStep => Error.Create(Code, "Error while handling step");
    public static Error WorkflowIsNotValid => Error.Create(Code, "Workflow is not valid");

    public static Error StepNotFound => Error.Create(Code, "Step not found");

    public static Error StepIsAtFirstPosition => Error.Create(Code, "Step is at first position");

    public static Error StepIsAtEndPosition => Error.Create(Code, "Step is at last position");

    public static Error WorkflowValidationFailed => Error.Create(Code, "Workflow validation failed");

    public static Error SubworkflowNotFound(string wfName) => Error.Create(Code, $"Subworkflow {wfName} not found");

    public static Error WorkflowNotFound(string filePath) => Error.Create(Code, $"Workflow not found at {filePath}");
}
