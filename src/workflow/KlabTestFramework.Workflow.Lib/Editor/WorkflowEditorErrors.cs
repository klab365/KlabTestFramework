using Klab.Toolkit.Results;

namespace KlabTestFramework.Workflow.Lib.Editor;

public static class WorkflowEditorErrors
{
    public static Error WorkflowIsNotValid => new(1, "Workflow is not valid");
}
