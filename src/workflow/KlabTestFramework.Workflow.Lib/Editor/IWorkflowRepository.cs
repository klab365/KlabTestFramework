﻿using System.Threading.Tasks;
using KlabTestFramework.Workflow.Lib.Specifications;

namespace KlabTestFramework.Workflow.Lib.Editor;

/// <summary>
/// Represents a repository for managing workflows.
/// </summary>
public interface IWorkflowRepository
{
    /// <summary>
    /// Retrieves a workflow from the repository asynchronously.
    /// </summary>
    /// <param name="path">The path of the workflow.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the retrieved workflow.</returns>
    public Task<WorkflowData> GetWorkflowAsync(string path);

    /// <summary>
    /// Saves a workflow to the repository asynchronously.
    /// </summary>
    /// <param name="path">The path where the workflow should be saved.</param>
    /// <param name="workflow">The workflow to be saved.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task SaveWorkflowAsync(string path, WorkflowData workflow);
}
