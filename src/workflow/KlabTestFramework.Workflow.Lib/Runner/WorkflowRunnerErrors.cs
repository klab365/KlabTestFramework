using Klab.Toolkit.Results;

namespace KlabTestFramework.Workflow.Lib;

public static class WorkflowRunnerErrors
{
    public static Error WorkflowHasErrors => new(10, "Workflow has errors");
}
