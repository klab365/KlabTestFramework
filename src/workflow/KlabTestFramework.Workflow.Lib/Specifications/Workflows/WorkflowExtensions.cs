using System.Linq;

namespace KlabTestFramework.Workflow.Lib.Specifications;

internal static class WorkflowExtensions
{
    public static WorkflowData ToData(this Workflow workflow)
    {
        WorkflowData data = new();

        data.Steps = workflow.Steps.Select(s => s.ToData()).ToList();

        if (workflow.Variables.Count > 0)
        {
            data.Variables = workflow.Variables.Select(v => v.ToData()).ToList();
        }

        if (workflow.Subworkflows.Count > 0)
        {
            data.Subworkflows = workflow.Subworkflows.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToData());
        }

        return data;
    }
}
