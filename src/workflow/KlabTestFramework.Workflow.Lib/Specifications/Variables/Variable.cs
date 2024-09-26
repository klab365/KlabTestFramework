using System;
using KlabTestFramework.Shared.Parameters;


namespace KlabTestFramework.Workflow.Lib.Specifications;

/// <summary>
/// Represents a variable in a workflow
/// </summary>
/// <typeparam name="TParameter"></typeparam>
public class Variable<TParameter> : IVariable where TParameter : IParameterType
{
    public string Name { get; set; } = string.Empty;

    public string Unit { get; set; } = string.Empty;

    public VariableType VariableType { get; set; } = VariableType.Constant;

    private TParameter? _parameter;
    public TParameter Parameter => _parameter ?? throw new InvalidOperationException("Parameter is not initialized.");

    public string DataType => Parameter.TypeKey;

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
