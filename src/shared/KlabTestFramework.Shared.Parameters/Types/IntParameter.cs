

using System;

namespace KlabTestFramework.Shared.Parameters.Types;

/// <summary>
/// Int parameter representation
/// </summary>
public class IntParameter : BaseParameterType<int>
{
    public override string AsString()
    {
        return Value.ToString();
    }

    public override void FromString(string data)
    {
        if (!int.TryParse(data, out int intValue))
        {
            throw new ArgumentException($"Value '{data}' is not a valid integer.");
        }

        SetValue(intValue);
    }
}
