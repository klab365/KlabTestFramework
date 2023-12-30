using System.Threading.Tasks;
using KlabTestFramework.Workflow.Lib.Specifications;

namespace KlabTestFramework.Workflow.Lib.Editor;

public interface IWorkflowRepository
{
    public Task<WorkflowData> GetWorkflowAsync(string path);

    public Task SaveWorkflowAsync(string path, WorkflowData workflow);
}
