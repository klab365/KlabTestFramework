using System;
using System.Collections.Generic;

namespace KlabTestFramework.Shared.Parameters.Types;

public abstract class BaseParameterType<TValue> : IParameterType<TValue>
{
    private readonly List<Func<TValue, bool>> _validaCallbacks = new();

    public virtual string TypeKey => GetType().Name;

    public string Name { get; set; } = string.Empty;

    public string Unit { get; set; } = string.Empty;

    private TValue? _value;

    public TValue Value => _value ?? throw new InvalidOperationException("Value not set");

    public IEnumerable<Func<TValue, bool>> ValidaCallbacks => _validaCallbacks;

    public event Action<TValue>? ValueChanged;

    public abstract string AsString();

    public abstract void FromString(string data);

    public bool IsValid()
    {
        return _validaCallbacks.TrueForAll(x => x(Value));
    }

    public void AddValidation(Func<TValue, bool> value)
    {
        _validaCallbacks.Add(value);
    }

    public virtual void SetValue(TValue newValue)
    {
        _value = newValue;
        ValueChanged?.Invoke(Value);
    }
}
