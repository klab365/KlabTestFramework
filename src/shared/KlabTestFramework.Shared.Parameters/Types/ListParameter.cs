using System.Collections.Generic;

namespace KlabTestFramework.Shared.Parameters.Types;

public class ListParameter<TParameterType>(ParameterFactory parameterFactory) : BaseParameterType<List<TParameterType>> where TParameterType : IParameterType
{
    private const string Separator = ",";
    private readonly ParameterFactory _parameterFactory = parameterFactory;

    public override string TypeKey => $"{nameof(ListParameter<TParameterType>)}<{typeof(TParameterType).Name}>";

    public override string AsString()
    {
        return string.Join(Separator, Value);
    }

    public override void FromString(string data)
    {
        string[] values = data.Split(Separator);
        Value.Clear();
        foreach (string value in values)
        {
            TParameterType parameter = _parameterFactory.CreateParameterType<TParameterType>();
            parameter.FromString(value);
            Value.Add(parameter);
        }
    }
}
