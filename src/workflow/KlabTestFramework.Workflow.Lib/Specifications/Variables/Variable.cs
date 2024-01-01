﻿using System;


namespace KlabTestFramework.Workflow.Lib.Specifications;

public class Variable<TParameter> : IVariable where TParameter : IParameterType
{
    public string Name { get; private set; } = string.Empty;

    public string Unit { get; private set; } = string.Empty;

    public VariableType VariableType { get; private set; } = VariableType.Constant;

    public TParameter? Parameter { get; private set; }

    public Type DataType => typeof(TParameter);

    /// <inheritdoc/>
    public void FromData(VariableData data)
    {
        Name = data.Name;
        Unit = data.Unit;
        VariableType = data.VariableType;
        Parameter?.FromString(data.Value ?? string.Empty);
    }

    /// <inheritdoc/>
    public VariableData ToData()
    {
        return new()
        {
            Name = Name,
            Unit = Unit,
            VariableType = VariableType,
            DataType = DataType.Name,
            Value = Parameter?.AsString() ?? string.Empty
        };
    }

    public void Init(IParameterType parameterType)
    {
        Parameter = (TParameter)parameterType;
    }
}
