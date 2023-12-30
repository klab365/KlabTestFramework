using System.Threading.Tasks;
using Klab.Toolkit.Results;

namespace KlabTestFramework.Workflow.Lib.Editor;

/// <summary>
/// Interface to read a workflow.
/// </summary>
public interface IWorkflowReadEditor
{
    /// <summary>
    /// Load a workflow from a file.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    Task<Result<Specifications.Workflow>> LoadWorkflowFromFileAsync(string path);
}
