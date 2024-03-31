

using System;

namespace KlabTestFramework.Shared.Parameters.Types;

public class BoolParameter : BaseParameterType<bool>
{
    public override string AsString()
    {
        return Value.ToString();
    }

    public override void FromString(string data)
    {
        if (!bool.TryParse(data, out bool boolValue))
        {
            throw new ArgumentException($"Value '{data}' is not a valid boolean.");
        }

        SetValue(boolValue);
    }
}
