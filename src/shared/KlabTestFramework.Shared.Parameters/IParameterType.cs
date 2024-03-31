using System;
using System.Collections.Generic;

namespace KlabTestFramework.Shared.Parameters;

/// <summary>
/// Represents the type of a parameter.
/// </summary>
public interface IParameterType
{
    /// <summary>
    /// Type key of the parameter
    /// </summary>
    string TypeKey { get; }

    /// <summary>
    /// Gets or sets the name of the parameter.
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// Gets or sets the unit of the parameter.
    /// </summary>
    string Unit { get; set; }

    /// <summary>
    /// Gets a value indicating whether the parameter is valid.
    /// </summary>
    bool IsValid();

    /// <summary>
    /// Gets the string representation of the parameter.
    /// </summary>
    /// <returns></returns>
    string AsString();

    /// <summary>
    /// Sets the parameter value from a string representation.
    /// </summary>
    /// <param name="data"></param>
    void FromString(string data);
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
    /// Gets the list of validation callbacks for the parameter type.
    /// </summary>
    IEnumerable<Func<TValue, bool>> ValidaCallbacks { get; }

    /// <summary>
    /// Adds a validation callback for the parameter type.
    /// </summary>
    /// <param name="value">The validation callback to add.</param>
    void AddValidation(Func<TValue, bool> value);

    /// <summary>
    /// Gets the type of the parameter value.
    /// </summary>
    Type ValueType => typeof(TValue);

    /// <summary>
    /// Sets the value of the parameter.
    /// </summary>
    /// <param name="newValue">The new value to set.</param>
    void SetValue(TValue newValue);

    /// <summary>
    /// Event raised when the value of the parameter changes.
    /// </summary>
    event Action<TValue>? ValueChanged;
}
