namespace KlabTestFramework.Shared.Parameters.Types;

public class StringParameter : BaseParameterType<string>
{
    public override string AsString()
    {
        return Value;
    }

    public override void FromString(string data)
    {
        SetValue(data);
    }
}
