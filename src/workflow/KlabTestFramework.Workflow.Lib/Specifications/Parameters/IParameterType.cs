using System;

namespace KlabTestFramework.Workflow.Lib;

/// <summary>
/// Represents the type of a parameter.
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
    void FromString(string data);

    /// <summary>
    /// Creates a deep copy of the parameter type.
    /// </summary>
    /// <returns>A new instance of the parameter type with the same values.</returns>
    IParameterType Clone();
}

/// <summary>
/// Represents a parameter with a specific value.
/// </summary>
/// <typeparam name="TValue">The type of the parameter value.</typeparam>
public interface IParameterType<TValue> : IParameterType
{
    /// <summary>
    /// Gets the value of the parameter.
    /// Set is done by <see cref="SetValue(TValue)"/> method.
    /// </summary>
    TValue Value { get; }

    /// <summary>
    /// Gets the type of the parameter value.
    /// </summary>
    Type ValueType => typeof(TValue);

    /// <summary>
    /// Sets the value of the parameter.
    /// </summary>
    /// <param name="newValue">The new value to set.</param>
    void SetValue(TValue newValue);

    event Action<TValue>? ValueChanged;
}

