using System;
using System.Threading.Tasks;
using Klab.Toolkit.Results;

using KlabTestFramework.Workflow.Lib.Specifications;

namespace KlabTestFramework.Workflow.Lib.Editor;

/// <summary>
/// Interface to manipulate a workflow.
/// </summary>
public interface IWorkflowEditor : IWorkflowReadEditor
{
    /// <summary>
    /// Save the workflow to a file.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="workflow"></param>
    /// <returns></returns>
    Task<Result> SaveWorkflowAsync(string path, Specifications.Workflow workflow);

    /// <summary>
    /// Start to create a new workflow
    /// </summary>
    void CreateNewWorkflow();

    /// <summary>
    /// Edit a existing workflow
    /// </summary>
    /// <param name="workflow"></param>
    void EditWorkflow(IWorkflow workflow);

    /// <summary>
    /// Adds a step to last position in the workflow.
    /// </summary>
    /// <typeparam name="TStep">The type of the step to add.</typeparam>
    /// <param name="configureCallback">An optional callback to configure the step.</param>
    void AddStep<TStep>(Action<TStep>? configureCallback = default) where TStep : IStep;

    /// <summary>
    /// Add variable to the workflow.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="configureCallback"></param>
    /// <typeparam name="TParameter"></typeparam>
    void AddVariable<TParameter>(string name, string unit, VariableType variableType, Action<TParameter>? configureCallback = default!) where TParameter : IParameterType;

    /// <summary>
    /// Configure the metadata of the workflow.
    /// </summary>
    /// <param name="metaDataConfigureCallback"></param>
    void ConfigureMetadata(Action<WorkflowData> metaDataConfigureCallback);

    /// <summary>
    /// Build the workflow with the current state of editor.
    /// </summary>
    /// <returns></returns>
    Result<Specifications.Workflow> BuildWorkflow();
}
