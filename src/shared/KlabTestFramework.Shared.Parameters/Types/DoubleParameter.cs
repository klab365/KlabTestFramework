

using System;

namespace KlabTestFramework.Shared.Parameters.Types;

public class DoubleParameter : BaseParameterType<double>
{
    public override string AsString()
    {
        return Value.ToString();
    }

    public override void FromString(string data)
    {
        if (!double.TryParse(data, out double doubleValue))
        {
            throw new ArgumentException($"Value '{data}' is not a valid double.");
        }

        SetValue(doubleValue);
    }
}
