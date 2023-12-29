using System;
using System.Collections.Generic;
using KlabTestFramework.Workflow.Lib.Contracts;

namespace KlabTestFramework.Workflow.Lib.Specifications;

/// <summary>
/// Represents a workflow that consists of multiple steps.
/// </summary>
public class Workflow
{
    private readonly List<StepContainer> _steps = new();
    private readonly IStepFactory _stepFactory;

    /// <summary>
    /// Gets or sets the description of the workflow.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets the date and time when the workflow was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets the date and time when the workflow was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Gets the read-only list of steps in the workflow.
    /// </summary>
    public IReadOnlyList<StepContainer> Steps => _steps.AsReadOnly();

    /// <summary>
    /// Initializes a new instance of the <see cref="Workflow"/> class.
    /// </summary>
    /// <param name="stepFactory">The step factory used to create steps.</param>
    public Workflow(IStepFactory stepFactory)
    {
        _stepFactory = stepFactory;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Adds a step to the workflow.
    /// </summary>
    /// <typeparam name="TStep">The type of the step to add.</typeparam>
    /// <param name="configureCallback">An optional callback to configure the step.</param>
    public void AddStep<TStep>(Action<TStep>? configureCallback = default) where TStep : IStep
    {
        IStep step = _stepFactory.CreateStep<TStep>();
        configureCallback?.Invoke((TStep)step);
        AddStep(step);
    }

    private void AddStep(IStep step)
    {
        StepContainer stepContainer = new(step) { OrderIndex = _steps.Count };
        _steps.Add(stepContainer);
    }
}

/// <summary>
/// Represents a container for a step in a workflow.
/// </summary>
public class StepContainer(IStep step)
{
    /// <summary>
    /// Gets or sets the order index.
    /// </summary>
    public int OrderIndex { get; set; }

    /// <summary>
    /// Gets the step of the workflow.
    /// </summary>
    public IStep Step { get; } = step;
}

