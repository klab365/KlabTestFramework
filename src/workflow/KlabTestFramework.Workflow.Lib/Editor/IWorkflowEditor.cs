using System;
using System.Threading.Tasks;
using Klab.Toolkit.Results;
using KlabTestFramework.Shared.Parameters;
using KlabTestFramework.Workflow.Lib.Specifications;

namespace KlabTestFramework.Workflow.Lib.Editor;

/// <summary>
/// Interface to manipulate a workflow.
/// </summary>
public interface IWorkflowEditor
{
    /// <summary>
    /// Save the workflow to a file.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="workflow"></param>
    /// <returns></returns>
    Task<Result> SaveWorkflowAsync(string path);

    /// <summary>
    /// Load a workflow from a file and set it as the current workflow for editing or building.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    Task<Result> LoadWorkflowFromFileAsync(string path);

    /// <summary>
    /// Start to create a new workflow
    /// At the end of the editing process, the <see cref="BuildWorkflowAsync"/> method
    /// </summary>
    void CreateNewWorkflow();

    /// <summary>
    /// Edit an existing workflow.
    /// At the end of the editing process, the <see cref="BuildWorkflowAsync"/> method
    /// must be called to build the new workflow.
    /// </summary>
    /// <param name="workflow"></param>
    void EditWorkflow(IWorkflow workflow);

    /// <summary>
    /// Adds a step to last position in the workflow.
    /// </summary>
    /// <typeparam name="TStep">The type of the step to add.</typeparam>
    /// <param name="configureCallback">An optional callback to configure the step.</param>
    TStep AddStepToLastPosition<TStep>(Action<TStep>? configureCallback = default) where TStep : IStep;

    /// <summary>
    /// Adds a step to the parent step of the workflow.
    /// </summary>
    /// <param name="parentStep"></param>
    /// <param name="childStep"></param>
    /// <returns></returns>
    Result AddChildStepToLastPosition(IStepWithChildren parentStep, string stepKey, Action<IStep>? configureCallback = default!);

    /// <summary>
    /// Move the step up in the workflow.
    /// </summary>
    Result MoveStepUp(IStep step);

    /// <summary>
    /// Move the child step up in the workflow.
    /// </summary>
    /// <param name="parentStep"></param>
    /// <param name="childStep"></param>
    /// <returns></returns>
    Result MoveChildStepUp(IStepWithChildren parentStep, IStep childStep);

    /// <summary>
    /// Move the step down in the workflow.
    /// </summary>
    Result MoveStepDown(IStep step);

    Result MoveChildStepDown(IStepWithChildren parentStep, IStep childStep);

    /// <summary>
    /// Add variable to the workflow.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="configureCallback"></param>
    /// <typeparam name="TParameter"></typeparam>
    void AddVariable<TParameter>(string name, string unit, VariableType variableType, Action<TParameter>? configureCallback = default!) where TParameter : IParameterType;

    /// <summary>
    /// Include a subworkflow in the workflow.
    /// </summary>
    /// <param name="workflow"></param>
    /// <param name="name"></param>
    void IncludeSubworkflow(string name, IWorkflow workflow);

    /// <summary>
    /// Configure the metadata of the workflow.
    /// </summary>
    /// <param name="metaDataConfigureCallback"></param>
    void ConfigureMetadata(Action<WorkflowData> metaDataConfigureCallback);

    /// <summary>
    /// Build the workflow with the current state of editor.
    /// </summary>
    /// <returns></returns>
    Task<Result<IWorkflow>> BuildWorkflowAsync();
}
