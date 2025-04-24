using Klab.Toolkit.Results;

namespace KlabTestFramework.Workflow.Abstractions;

public static class WorkflowModuleErrors
{
    public static Error WorkflowHasErrors => Error.Create("Workflow", "Workflow has errors");

    public static Error ErrorWhileHandlingStep => Error.Create("Workflow", "Error while handling step");
    public static Error WorkflowIsNotValid => Error.Create("Workflow", "Workflow is not valid");

    public static Error StepNotFound => Error.Create("Workflow", "Step not found");

    public static Error StepIsAtFirstPosition => Error.Create("Workflow", "Step is at first position");

    public static Error StepIsAtEndPosition => Error.Create("Workflow", "Step is at last position");

    public static Error WorkflowValidationFailed => Error.Create("Workflow", "Workflow validation failed");

    public static Error SubworkflowNotFound(string wfName) => Error.Create("Workflow", $"Subworkflow {wfName} not found");

    public static Error WorkflowNotFound(string filePath) => Error.Create("Workflow", $"Workflow not found at {filePath}");
}
