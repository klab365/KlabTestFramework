using Klab.Toolkit.Results;

namespace KlabTestFramework.Workflow.Lib.Editor;

public static class WorkflowEditorErrors
{
    public static InformativeError WorkflowIsNotValid => new(string.Empty, "Workflow is not valid");

    public static InformativeError StepNotFound => new(string.Empty, "Step not found");

    public static InformativeError StepIsAtFirstPosition => new(string.Empty, "Step is at first position");

    public static InformativeError StepIsAtEndPosition => new(string.Empty, "Step is at last position");
}
