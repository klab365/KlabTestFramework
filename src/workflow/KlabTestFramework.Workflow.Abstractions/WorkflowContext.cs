using System;
using System.Collections.Generic;
using KlabTestFramework.Shared.Parameters;

namespace KlabTestFramework.Workflow.Abstractions.Specifications;

/// <summary>
/// Default implementation of <see cref="IWorkflowContext"/> interface.
/// </summary>
public class WorkflowContext
{
    private readonly Dictionary<string, IParameterType> _runTimeVariables = [];
    public IReadOnlyDictionary<string, IParameterType> RunTimeVariables => _runTimeVariables;

    /// <summary>
    /// Adds a runtime variable to the workflow context.
    /// This is useful for storing temporary data that is needed during the workflow execution.
    /// The key must be unique within the context, and if a variable with the same key already exists,
    /// </summary>
    public void AddRunTimeVariable<TParameter>(string key, TParameter value) where TParameter : IParameterType
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Key cannot be null or whitespace.", nameof(key));
        }

        if (_runTimeVariables.ContainsKey(key))
        {
            throw new ArgumentException($"Key '{key}' already exists in the runtime variables.", nameof(key));
        }

        _runTimeVariables.Add(key, value);
    }
}
