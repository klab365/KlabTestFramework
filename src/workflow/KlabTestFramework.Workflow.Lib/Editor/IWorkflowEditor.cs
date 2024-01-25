﻿using System;
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
    /// Edit an existing workflow.
    /// At the end of the editing process, the <see cref="BuildWorkflowAsync"/> method
    /// must be called to build the new workflow.
    /// </summary>
    /// <param name="workflow"></param>
    void EditWorkflow(Specifications.Workflow workflow);

    /// <summary>
    /// Adds a step to last position in the workflow.
    /// </summary>
    /// <typeparam name="TStep">The type of the step to add.</typeparam>
    /// <param name="configureCallback">An optional callback to configure the step.</param>
    TStep AddStep<TStep>(Action<TStep>? configureCallback = default) where TStep : IStep;

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
    Task<Result<Specifications.Workflow>> BuildWorkflowAsync();

    /// <summary>
    /// Check if the workflow has errors.
    /// </summary>
    Task<Result> CheckWorkflowHasErrorsAsync(Specifications.Workflow workflow);
}
