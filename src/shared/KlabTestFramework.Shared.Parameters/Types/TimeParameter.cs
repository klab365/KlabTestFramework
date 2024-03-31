

using System;
using System.Globalization;

namespace KlabTestFramework.Shared.Parameters.Types;

/// <summary>
/// Time parameter representation
/// </summary>
public class TimeParameter : BaseParameterType<TimeSpan>
{
    public override string AsString()
    {
        return Value.ToString();
    }

    public override void FromString(string data)
    {
        if (!TimeSpan.TryParse(data, CultureInfo.InvariantCulture, out TimeSpan timeSpanValue))
        {
            throw new ArgumentException($"Invalid time format: {data}");
        }

        SetValue(timeSpanValue);
    }
}
