namespace KlabTestFramework.Workflow.Lib.Specifications;


public abstract class Parameter : IParameter
{
    /// <summary>
    /// Gets the display name of the parameter.
    /// </summary>
    public string DisplayName { get; }

    /// <summary>
    /// Gets the unit of the parameter.
    /// </summary>
    public string Unit { get; }

    /// <summary>
    /// Gets the variable name of the parameter.
    /// </summary>
    public string VariableName { get; set; } = string.Empty;

    /// <summary>
    /// Gets the value type of the parameter.
    /// </summary>
    public bool IsVariable { get; private set; }

    protected Parameter(string displayName, string unit)
    {
        DisplayName = displayName;
        Unit = unit;
    }

    public void ChangetToVariable(string variableName)
    {
        VariableName = variableName;
        IsVariable = true;
    }

    public void ChangeToValue()
    {
        IsVariable = false;
        VariableName = string.Empty;
    }

    /// <summary>
    /// Abstract method to get the content of the parameter as string.
    /// </summary>
    /// <returns></returns>
    public abstract string ContentAsString();

    public string ToData()
    {
        string value = IsVariable ? VariableName : ContentAsString();
        return value;
    }
}

public class Parameter<TParameter> : Parameter where TParameter : IParameterType
{
    /// <summary>
    /// Content of the parameter.
    /// </summary>
    public TParameter Content { get; }

    public Parameter(string displayName, string unit, TParameter content) : base(displayName, unit)
    {
        Content = content;
    }

    /// <summary>
    /// Content of the parameter as string
    /// </summary>
    /// <returns></returns>
    public override string ContentAsString()
    {
        return Content.AsString();
    }
}
