using System;
using KlabTestFramework.Shared.Parameters;


namespace KlabTestFramework.Workflow.Lib.Specifications;

/// <summary>
/// Represents a variable in a workflow
/// </summary>
/// <typeparam name="TParameter"></typeparam>
public class Variable<TParameter> : IVariable where TParameter : IParameterType
{
    public string Name { get; private set; } = string.Empty;

    public string Unit { get; private set; } = string.Empty;

    public VariableType VariableType { get; private set; } = VariableType.Constant;

    private TParameter? _parameter;
    public TParameter Parameter => _parameter ?? throw new InvalidOperationException("Parameter is not initialized.");

    public string DataType => Parameter.TypeKey;

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
            DataType = DataType,
            Value = Parameter.AsString() ?? string.Empty
        };
    }

    public void Init(IParameterType parameterType)
    {
        _parameter = (TParameter)parameterType;
    }

    public IParameterType GetParameterType()
    {
        if (Parameter is null)
        {
            throw new InvalidOperationException("Parameter is not initialized.");
        }
        return Parameter;
    }

    public void UpdateValue(string value)
    {
        Parameter?.FromString(value);
    }
}
