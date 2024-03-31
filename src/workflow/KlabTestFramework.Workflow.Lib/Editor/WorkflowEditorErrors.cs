using Klab.Toolkit.Results;

namespace KlabTestFramework.Workflow.Lib.Editor;

public static class WorkflowEditorErrors
{
    public static Error WorkflowIsNotValid => new(1, "Workflow is not valid");

    public static Error StepNotFound => new(2, "Step not found");

    public static Error StepIsAtFirstPosition => new(3, "Step is at first position");

    public static Error StepIsAtEndPosition => new(4, "Step is at last position");
}
