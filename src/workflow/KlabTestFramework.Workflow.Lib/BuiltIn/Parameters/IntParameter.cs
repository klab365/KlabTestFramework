

using System;
using System.Collections.Generic;
using KlabTestFramework.Workflow.Lib.Specifications;

namespace KlabTestFramework.Workflow.Lib.BuiltIn;

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

    public void FromString(string data)
    {
        if (!int.TryParse(data, out int intValue))
        {
            throw new ArgumentException($"Value '{data}' is not a valid integer.");
        }

        SetValue(intValue);
    }

    public void AddValiation(Func<int, bool> value)
    {
        ValidaCallbacks.Add(value);
    }
}
