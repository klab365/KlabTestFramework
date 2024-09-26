using System;
using System.Collections.Generic;
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
/// Represents a step in a workflow that has children.
/// </summary>
public interface IStepWithChildren : IStep
{
    List<IStep> Children { get; }
}

/// <summary>
/// Represents a step in a workflow that is a subworkflow.
/// </summary>
public interface ISubworkflowStep : IStepWithChildren
{
    Parameter<SelectableParameter<StringParameter>> SelectedSubworkflow { get; }

    event Action<string>? SubworkflowSelected;

    Workflow Subworkflow { get; set; }

    List<IParameter> Arguments { get; }
}

