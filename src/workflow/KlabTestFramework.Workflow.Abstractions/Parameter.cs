using System;
using KlabTestFramework.Shared.Parameters;

namespace KlabTestFramework.Workflow.Lib.Specifications;

/// <summary>
/// Represents a parameter in the workflow specification. A parameter can be inside a step or in the variables
/// </summary>
public class Parameter<TParameter> : IParameter where TParameter : IParameterType
{
    /// <inheritdoc/>
    public string Name { get; set; }

    /// <inheritdoc/>
    public string Unit { get; set; }

    /// <inheritdoc/>
    public string VariableName { get; set; } = string.Empty;

    /// <inheritdoc/>
    public ParameterValueType ParameterType { get; set; }

    /// <summary>
    /// Content of the parameter.
    /// </summary>
    public TParameter Content { get; }

    public Type ParameterContentType
    {
        get
        {
            if (Content is not IParameterType)
            {
                throw new InvalidOperationException($"Parameter content must implement {nameof(IParameterType)}");
            }

            return typeof(TParameter);
        }
    }

    public Parameter(string name, string unit, TParameter content)
    {
        Content = content;
        Name = name;
        Unit = unit;
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

    public bool IsVariable()
    {
        return ParameterType == ParameterValueType.Variable;
    }

    public IParameterType GetParameterType()
    {
        return Content;
    }
}
