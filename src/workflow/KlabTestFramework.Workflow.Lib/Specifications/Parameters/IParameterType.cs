﻿using System;
using KlabTestFramework.Workflow.Lib.Specifications;

namespace KlabTestFramework.Workflow.Lib;

/// <summary>
/// Represents a parameter in the workflow.
/// </summary>
public interface IParameterType
{
    /// <summary>
    /// Checks if the parameter is valid.
    /// </summary>
    /// <returns>True if the parameter is valid; otherwise, false.</returns>
    bool IsValid();

    /// <summary>
    /// String representation of the parameter.
    /// </summary>
    /// <returns></returns>
    string AsString();

    /// <summary>
    /// Converts a string representation of a value to the corresponding parameter type.
    /// </summary>
    /// <param name="data">Data which is deserialized</param>
    void FromData(ParameterData data);
}

/// <summary>
/// Represents a parameter with a specific value.
/// </summary>
/// <typeparam name="TValue">The type of the parameter value.</typeparam>
public interface IParameterType<TValue> : IParameterType
{
    /// <summary>
    /// Gets the value of the parameter.
    /// Set is done by <see cref="SetValue(TValue)"/>.
    /// </summary>
    TValue? Value { get; }

    /// <summary>
    /// Gets the type of the parameter value.
    /// </summary>
    Type ValueType => typeof(TValue);

    /// <summary>
    /// Sets the value of the parameter.
    /// </summary>
    /// <param name="newValue">The new value to set.</param>
    void SetValue(TValue newValue);
}
