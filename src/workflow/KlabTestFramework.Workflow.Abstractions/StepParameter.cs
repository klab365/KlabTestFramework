using System;
using KlabTestFramework.Shared.Parameters;

namespace KlabTestFramework.Workflow.Abstractions.Specifications;

/// <summary>
/// Represents a parameter in the workflow specification. A parameter can be inside a step or in the variables
/// </summary>
public class StepParameter<TParameterType> : IStepParameter where TParameterType : IParameterType
{
    /// <inheritdoc/>
    public string Name { get; set; }

    /// <inheritdoc/>
    public string Unit { get; set; }

    /// <inheritdoc/>
    public string VariableName { get; set; } = string.Empty;

    /// <inheritdoc/>
    public StepParameterValueType ParameterType { get; set; }

    /// <summary>
    /// Content of the parameter.
    /// </summary>
    public TParameterType Content { get; }

    public Type ParameterContentType
    {
        get
        {
            if (Content is not IParameterType)
            {
                throw new InvalidOperationException($"Parameter content must implement {nameof(IParameterType)}");
            }

            return typeof(TParameterType);
        }
    }

    public StepParameter(string name, string unit, TParameterType content)
    {
        Content = content;
        Name = name;
        Unit = unit;
    }

    /// <inheritdoc/>
    public void ChangetToVariable(string variableName)
    {
        ParameterType = StepParameterValueType.Variable;
        VariableName = variableName;
    }

    /// <inheritdoc/>
    public void ChangeToValue()
    {
        ParameterType = StepParameterValueType.Value;
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
        return ParameterType == StepParameterValueType.Variable;
    }

    public IParameterType GetParameterType()
    {
        return Content;
    }
}
