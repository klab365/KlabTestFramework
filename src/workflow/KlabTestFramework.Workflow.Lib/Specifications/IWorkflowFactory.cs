namespace KlabTestFramework.Workflow.Lib.Specifications;

/// <summary>
/// Represents a factory for creating instances of Workflow.
/// </summary>
public interface IWorkflowFactory
{
    /// <summary>
    /// Creates a new instance of Workflow.
    /// </summary>
    /// <returns>A new instance of Workflow.</returns>
    Workflow CreateWorkflow();
}
