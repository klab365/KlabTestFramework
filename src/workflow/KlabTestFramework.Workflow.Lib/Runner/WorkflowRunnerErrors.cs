using Klab.Toolkit.Results;

namespace KlabTestFramework.Workflow.Lib;

public static class WorkflowRunnerErrors
{
    public static InformativeError WorkflowHasErrors => new(string.Empty, "Workflow has errors");

    public static InformativeError ErrorWhileHandlingStep => new(string.Empty, "Error while handling step");
}
