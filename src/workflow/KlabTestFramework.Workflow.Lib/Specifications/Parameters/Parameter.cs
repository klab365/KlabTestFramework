namespace KlabTestFramework.Workflow.Lib.Specifications;

/// <summary>
/// Represents a parameter in the workflow specification. A parameter can be inside a step or in the variables
/// </summary>
public class Parameter<TParameter> : IParameter where TParameter : IParameterType
{
    /// <inheritdoc/>
    public string Name { get; private set; }

    /// <inheritdoc/>
    public string Unit { get; private set; }

    /// <inheritdoc/>
    public string VariableName { get; private set; } = string.Empty;

    /// <inheritdoc/>
    public ParameterValueType ParameterType { get; private set; }

    /// <summary>
    /// Content of the parameter.
    /// </summary>
    public TParameter Content { get; }

    public Parameter(string name, string unit, TParameter content)
    {
        Name = name;
        Unit = unit;
        Content = content;
    }

    /// <inheritdoc/>
    public void ChangetToVariable(string variableName)
    {
        ParameterType = ParameterValueType.Variable;
        VariableName = variableName;
    }

    /// <inheritdoc/>
    public void ChangeToValue()
    {
        ParameterType = ParameterValueType.Value;
        VariableName = string.Empty;
    }

    /// <summary>
    /// Content of the parameter as string
    /// </summary>
    /// <returns></returns>
    public string ContentAsString()
    {
        string value = IsVariable() ? VariableName : Content.AsString();
        return value;
    }

    /// <inheritdoc/>
    public bool IsValid()
    {
        return Content.IsValid();
    }

    /// <inheritdoc/>
    public ParameterData ToData()
    {
        ParameterData data = new()
        {
            Name = Name,
            Type = ParameterType,
            Value = ContentAsString()
        };

        return data;
    }

    /// <inheritdoc/>
    public void FromData(ParameterData data)
    {
        Name = data.Name;
        ParameterType = data.Type;
        Content.FromString(data.Value);
    }

    private bool IsVariable()
    {
        return ParameterType == ParameterValueType.Variable;
    }
}
