using System.Collections.Generic;

namespace KlabTestFramework.Shared.Parameters.Types;

/// <summary>
/// Represents a parameter that can be selected from a list of options.
///
/// Value is the selected option.
/// </summary>
/// <typeparam name="TParameterType"></typeparam>
public class SelectableParameter<TParameterType> : BaseParameterType<TParameterType> where TParameterType : IParameterType
{
    private readonly ParameterFactory _parameterFactory;

    public override string TypeKey => $"{nameof(SelectableParameter<TParameterType>)}<{typeof(TParameterType).Name}>";

    public List<TParameterType> Options { get; } = new();

    public SelectableParameter(ParameterFactory parameterFactory)
    {
        AddValidation(o => Options.Exists(oo => o.AsString() == oo.AsString()));
        _parameterFactory = parameterFactory;
    }

    public override string AsString()
    {
        return Value.ToString() ?? string.Empty;
    }

    public override void FromString(string data)
    {
        TParameterType parameter = _parameterFactory.CreateParameterType<TParameterType>();
        parameter.FromString(data);
        SetValue(parameter);
    }

    public void AddOption(TParameterType option)
    {
        Options.Add(option);
    }

    public void AddOptions(string[] subworkflowNames)
    {
        foreach (string subworkflowName in subworkflowNames)
        {
            TParameterType parameter = _parameterFactory.CreateParameterType<TParameterType>();
            parameter.FromString(subworkflowName);
            Options.Add(parameter);
        }
    }

    public void SelectOption(TParameterType option)
    {
        SetValue(option);
    }
}
