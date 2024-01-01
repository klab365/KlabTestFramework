

using System;
using System.Collections.Generic;
using System.Globalization;

namespace KlabTestFramework.Workflow.Lib.Specifications;

/// <summary>
/// Int parameter representation
/// </summary>
public class IntParameter : IParameterType<int>, IWithValidation<int>
{
    public int Value { get; private set; }

    public List<Func<int, bool>> ValidaCallbacks { get; } = new();

    public bool IsValid()
    {
        return ValidaCallbacks.TrueForAll(v => v(Value));
    }

    public void SetValue(int newValue)
    {
        Value = newValue;
    }

    public string AsString()
    {
        return Value.ToString();
    }

    public void FromData(ParameterData data)
    {
        if (!int.TryParse(data.Value, out int intValue))
        {
            throw new ArgumentException($"Value '{data.Value}' is not a valid integer.");
        }

        SetValue(intValue);
    }

    public void AddValiation(Func<int, bool> value)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Time parameter representation
/// </summary>
public class TimeParameter : IParameterType<TimeSpan>, IWithValidation<TimeSpan>
{
    public TimeSpan Value { get; private set; }

    public List<Func<TimeSpan, bool>> ValidaCallbacks { get; } = new();

    public bool IsValid()
    {
        return ValidaCallbacks.TrueForAll(v => v(Value));
    }

    public void SetValue(TimeSpan newValue)
    {
        Value = newValue;
    }

    public void AddValiation(Func<TimeSpan, bool> value)
    {
        ValidaCallbacks.Add(value);
    }

    public string AsString()
    {
        return Value.ToString();
    }

    public void FromData(ParameterData data)
    {
        if (!TimeSpan.TryParse(data.Value, CultureInfo.InvariantCulture, out TimeSpan timeSpanValue))
        {
            throw new ArgumentException($"Value {data.Value} is not a valid TimeSpan.");
        }

        SetValue(timeSpanValue);
    }
}

/// <summary>
/// Represents an interface for objects that support validation of a specific parameter type.
/// </summary>
/// <typeparam name="TParameterType">The type of the parameter.</typeparam>
public interface IWithValidation<TParameterType>
{
    /// <summary>
    /// Gets the list of validation callbacks for the parameter type.
    /// </summary>
    List<Func<TParameterType, bool>> ValidaCallbacks { get; }

    /// <summary>
    /// Adds a validation callback for the parameter type.
    /// </summary>
    /// <param name="value">The validation callback to add.</param>
    void AddValiation(Func<TParameterType, bool> value);
}
