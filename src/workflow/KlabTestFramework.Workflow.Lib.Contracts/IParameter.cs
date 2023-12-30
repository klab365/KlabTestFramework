using System;

namespace KlabTestFramework.Workflow.Lib.Contracts;

/// <summary>
/// Represents a parameter in the workflow.
/// </summary>
public interface IParameter
{
    /// <summary>
    /// Gets the display name of the parameter.
    /// </summary>
    string DisplayName { get; }

    /// <summary>
    /// Gets the unit of the parameter.
    /// </summary>
    string Unit { get; }

    /// <summary>
    /// Gets the variable name of the parameter.
    /// </summary>
    string VariableName { get; }

    /// <summary>
    /// Gets the value type of the parameter.
    /// </summary>
    ParameterValueType ParameterValueType { get; }

    /// <summary>
    /// Checks if the parameter is valid.
    /// </summary>
    /// <returns>True if the parameter is valid; otherwise, false.</returns>
    bool IsValid();
}

/// <summary>
/// Represents a parameter with a specific value.
/// </summary>
/// <typeparam name="TValue">The type of the parameter value.</typeparam>
public interface IParameter<TValue> : IParameter
{
    /// <summary>
    /// Gets or sets the value of the parameter.
    /// </summary>
    TValue? Value { get; }

    /// <summary>
    /// Gets the .NET type of the parameter value.
    /// </summary>
    Type ValueType => typeof(TValue);

    /// <summary>
    /// Sets the value of the parameter.
    /// </summary>
    /// <param name="newValue">The new value to set.</param>
    void SetValue(TValue newValue);
}

/// <summary>
/// Represents the type of a parameter value.
/// </summary>
public enum ParameterValueType
{
    /// <summary>
    /// Represents a parameter with a specific value.
    /// </summary>
    Value
}

