using System;
using Klab.Toolkit.Results;

namespace KlabTestFramework.Workflow.Lib;

public static class WorkflowModuleErrors
{
    public static InformativeError WorkflowHasErrors => new(string.Empty, "Workflow has errors");

    public static InformativeError ErrorWhileHandlingStep => new(string.Empty, "Error while handling step");
    public static InformativeError WorkflowIsNotValid => new("Workflow", "Workflow is not valid");

    public static InformativeError StepNotFound => new("Workflow", "Step not found");

    public static InformativeError StepIsAtFirstPosition => new("Workflow", "Step is at first position");

    public static InformativeError StepIsAtEndPosition => new("Workflow", "Step is at last position");

    public static InformativeError SubworkflowNotFound(string wfName) => new("Workflow", $"Subworkflow {wfName} not found");

    public static InformativeError WorkflowNotFound(string filePath) => new("Workflow", $"Workflow not found at {filePath}");
}
