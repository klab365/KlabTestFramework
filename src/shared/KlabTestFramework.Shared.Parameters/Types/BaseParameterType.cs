using System;

namespace KlabTestFramework.Shared.Parameters.Types;

public abstract class BaseParameterType<TValue> : IParameterType<TValue>
{
    public virtual string TypeKey => GetType().Name;

    public string Name { get; set; } = string.Empty;

    public string Unit { get; set; } = string.Empty;

    private TValue? _value;

    public TValue Value => _value ?? throw new InvalidOperationException("Value not set");

    public event Action<TValue>? ValueChanged;

    public abstract string AsString();

    public abstract void FromString(string data);

    public virtual void SetValue(TValue newValue)
    {
        _value = newValue;
        ValueChanged?.Invoke(Value);
    }
}
