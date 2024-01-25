

using System;
using System.Collections.Generic;
using KlabTestFramework.Workflow.Lib.Specifications;

namespace KlabTestFramework.Workflow.Lib.BuiltIn;

public class StringParameter : IParameterType<string>, IWithValidation<string>
{
    public string Value { get; private set; } = string.Empty;

    public List<Func<string, bool>> ValidaCallbacks { get; } = new();

    public event Action<string>? ValueChanged;

    public string AsString()
    {
        return Value;
    }

    public IParameterType Clone()
    {
        StringParameter clonedParameter = (StringParameter)MemberwiseClone();
        clonedParameter.ValidaCallbacks.Clear();
        foreach (Func<string, bool> callback in ValidaCallbacks)
        {
            clonedParameter.ValidaCallbacks.Add(callback);
        }

        return clonedParameter;
    }

    public void FromString(string data)
    {
        SetValue(data);
    }

    public bool IsValid()
    {
        return ValidaCallbacks.TrueForAll(v => v(Value));
    }

    public void SetValue(string newValue)
    {
        Value = newValue;
        ValueChanged?.Invoke(Value);
    }
}
