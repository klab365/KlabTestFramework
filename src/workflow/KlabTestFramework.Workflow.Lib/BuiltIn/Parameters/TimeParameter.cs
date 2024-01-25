﻿

using System;
using System.Collections.Generic;
using System.Globalization;
using KlabTestFramework.Workflow.Lib.Specifications;

namespace KlabTestFramework.Workflow.Lib.BuiltIn;

/// <summary>
/// Time parameter representation
/// </summary>
public class TimeParameter : IParameterType<TimeSpan>, IWithValidation<TimeSpan>
{
    public TimeSpan Value { get; private set; }

    public List<Func<TimeSpan, bool>> ValidaCallbacks { get; } = new();

    public event Action<TimeSpan>? ValueChanged;

    public bool IsValid()
    {
        return ValidaCallbacks.TrueForAll(v => v(Value));
    }

    public void SetValue(TimeSpan newValue)
    {
        Value = newValue;
        ValueChanged?.Invoke(Value);
    }

    public void AddValiation(Func<TimeSpan, bool> value)
    {
        ValidaCallbacks.Add(value);
    }

    public string AsString()
    {
        return Value.ToString();
    }

    public void FromString(string data)
    {
        if (!TimeSpan.TryParse(data, CultureInfo.InvariantCulture, out TimeSpan timeSpanValue))
        {
            SetValue(TimeSpan.Zero); // todo: do it better (throw exception?)
        }

        SetValue(timeSpanValue);
    }

    public IParameterType Clone()
    {
        TimeParameter clonedParameter = (TimeParameter)MemberwiseClone();
        clonedParameter.ValidaCallbacks.Clear();
        foreach (Func<TimeSpan, bool> callback in ValidaCallbacks)
        {
            clonedParameter.ValidaCallbacks.Add(callback);
        }

        return clonedParameter;
    }
}
