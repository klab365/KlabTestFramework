using System;
using System.Collections.Generic;
using KlabTestFramework.Workflow.Lib.Contracts;

namespace KlabTestFramework.Workflow.Lib.Specifications.Parameters.ParameterTypes;

/// <summary>
/// Represents a parameter with a set of predefined choices.
/// </summary>
/// <typeparam name="TValue">The type of the parameter value.</typeparam>
public class ChoicesParameter<TValue> : IParameter<TValue>
{
    /// <summary>
    /// List of choices
    /// </summary>
    /// <returns></returns>
    public List<TValue> Choices { get; } = new();

    /// <inheritdoc/>
    public TValue? Value { get; private set; }

    /// <inheritdoc/>
    public string DisplayName { get; private set; } = string.Empty;

    /// <inheritdoc/>
    public string Unit { get; private set; } = string.Empty;

    /// <inheritdoc/>
    public string VariableName { get; private set; } = string.Empty;

    /// <inheritdoc/>
    public ParameterValueType ParameterValueType { get; private set; }

    /// <inheritdoc/>
    public bool IsValid()
    {
        if (Value == null)
        {
            return false;
        }

        return ContiainsChoise(Value);
    }

    /// <inheritdoc/>
    public void SetValue(TValue newValue)
    {
        if (!ContiainsChoise(newValue))
        {
            throw new ArgumentException($"The value {newValue} does not contain in the Choices.");
        }

        Value = newValue;
    }

    /// <summary>
    /// Initialize a new instance of the <see cref="ChoicesParameter{TValue}"/> class.
    /// The value of the parameter will be set to the first choice.
    /// </summary>
    /// <param name="displayName"></param>
    /// <param name="unit"></param>
    /// <param name="choices"></param>
    public void Init(string displayName, string unit, TValue[] choices)
    {
        DisplayName = displayName;
        Unit = unit;
        Choices.Clear();
        Choices.AddRange(choices);
        Value = choices[0];
    }

    private bool ContiainsChoise(TValue newValue)
    {
        return Choices.Contains(newValue);
    }
}
