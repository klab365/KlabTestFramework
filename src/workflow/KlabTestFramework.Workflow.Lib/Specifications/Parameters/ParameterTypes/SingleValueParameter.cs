using System;
using System.Collections.Generic;
using KlabTestFramework.Workflow.Lib.Contracts;

namespace KlabTestFramework.Workflow.Lib.Specifications.Parameters.ParameterTypes;

/// <summary>
/// Represents a single value parameter that implements the <see cref="IParameter{TValue}"/> interface.
/// </summary>
/// <typeparam name="TValue">The type of the parameter value.</typeparam>
public class SingleValueParameter<TValue> : IParameter<TValue>
{
    private readonly List<Func<TValue, bool>> _isValidCallbacks = new();

    /// <inheritdoc/>
    public TValue? Value { get; protected set; }

    /// <inheritdoc/>
    public string VariableName { get; private set; } = string.Empty;

    /// <inheritdoc/>
    public string DisplayName { get; private set; } = string.Empty;

    /// <inheritdoc/>
    public string Unit { get; private set; } = string.Empty;

    /// <inheritdoc/>
    public ParameterValueType ParameterValueType { get; private set; }

    /// <inheritdoc/>
    public bool IsValid()
    {
        if (Value == null)
        {
            return false;
        }

        return IsValueValid(Value);
    }

    /// <inheritdoc/>
    public void SetValue(TValue newValue)
    {
        bool isValid = IsValueValid(newValue);
        if (!isValid)
        {
            throw new ArgumentException($"The value {newValue} is not valid for the parameter {DisplayName}.");
        }

        Value = newValue;
    }

    /// <inheritdoc/>
    public void Init(string displayName, string unit, TValue? defaultValue, Func<TValue, bool>[] isValidCallbacks)
    {
        DisplayName = displayName;
        Unit = unit;
        Value = defaultValue;
        ParameterValueType = ParameterValueType.Value;

        if (_isValidCallbacks.Count > 0)
        {
            _isValidCallbacks.Clear();
        }
        _isValidCallbacks.AddRange(isValidCallbacks);
    }

    private bool IsValueValid(TValue value)
    {
        foreach (Func<TValue, bool> isValidCallback in _isValidCallbacks)
        {
            if (!isValidCallback(value))
            {
                return false;
            }
        }

        return true;
    }
}
