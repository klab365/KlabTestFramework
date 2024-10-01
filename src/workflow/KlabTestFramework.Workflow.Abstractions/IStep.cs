using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Klab.Toolkit.Results;
using KlabTestFramework.Shared.Parameters.Types;

namespace KlabTestFramework.Workflow.Lib.Specifications;

/// <summary>
/// Represents a step in a workflow.
/// </summary>
public interface IStep
{
    StepId Id { get; set; }

    IEnumerable<IParameter> GetParameters();
}

/// <summary>
/// Represents a step in a workflow that is a subworkflow.
/// </summary>
public interface ISubworkflowStep : IStep
{
    /// <summary>
    /// Gets the selected subworkflow.
    /// </summary>
    Parameter<SelectableParameter<StringParameter>> SelectedSubworkflow { get; }

    /// <summary>
    /// Gets the subworkflow.
    /// </summary>
    Workflow Subworkflow { get; }

    /// <summary>
    /// Gets the arguments of the subworkflow.
    /// </summary>
    IEnumerable<IParameter> Arguments { get; }

    /// <summary>
    /// Gets the steps of the subworkflow.
    /// </summary>
    IEnumerable<IStep> Steps { get; }

    /// <summary>
    /// Update the subworkflow content
    /// </summary>
    /// <param name="wfName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Result> UpdateSubworkflowAsync(string wfName, CancellationToken cancellationToken = default);

    /// <summary>
    /// simple wrapper
    /// </summary>
    /// <param name="subworkflows"></param>
    void AddSubworkflowOptions(params string[] subworkflows);

    /// <summary>
    /// simple wrapper
    /// </summary>
    /// <param name="subworkflows"></param>
    void RemoveSubworkflowOptions(params string[] subworkflows);
}

