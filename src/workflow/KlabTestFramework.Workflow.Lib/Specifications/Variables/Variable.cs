using System;


namespace KlabTestFramework.Workflow.Lib.Specifications;

public class Variable<TParameter> : IVariable where TParameter : IParameterType
{
    public string Name { get; set; }

    public string Unit { get; set; } = string.Empty;

    public VariableType VariableType { get; set; }

    public TParameter Parameter { get; }

    public Type DataType => typeof(TParameter);

    public Variable(string name, TParameter parameter)
    {
        Name = name;
        Parameter = parameter;
    }

    public void FromData(VariableData data)
    {
        Name = data.Name;
        Unit = data.Unit;
        VariableType = data.VariableType;
        Parameter.FromData(new() { Value = data.Value });
    }

    public VariableData ToData()
    {
        return new()
        {
            Name = Name,
            Unit = Unit,
            VariableType = VariableType,
            DataType = DataType.Name,
            Value = Parameter.AsString()
        };
    }
}
