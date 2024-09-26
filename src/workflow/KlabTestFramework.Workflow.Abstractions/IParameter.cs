using System;
using KlabTestFramework.Shared.Parameters;

namespace KlabTestFramework.Workflow.Lib.Specifications;

/// <summary>
/// Represents a parameter in the workflow specification.
/// </summary>
public interface IParameter
{
    /// <summary>
    /// Gets the name of the parameter.
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// Gets the unit of the parameter.
    /// </summary>
    string Unit { get; set; }

    /// <summary>
    /// Gets or sets the variable name of the parameter.
    /// </summary>
    string VariableName { get; set; }

    /// <summary>
    /// Gets a value indicating what type of parameter it is.
    ParameterValueType ParameterType { get; set; }

    /// <summary>
    /// Parameter content type
    /// </summary>
    /// <value></value>
    Type ParameterContentType { get; }

    /// <summary>
    /// Changes the parameter to a variable with the specified variable name.
    /// </summary>
    /// <param name="variableName">The name of the variable.</param>
    void ChangetToVariable(string variableName);

    /// <summary>
    /// Changes the parameter to a value.
    /// </summary>
    void ChangeToValue();

    /// <summary>
    /// Gets the content of the parameter as a string.
    /// </summary>
    /// <returns>The content of the parameter as a string.</returns>
    string ContentAsString();

    /// <summary>
    /// Checks if the parameter is valid.
    /// </summary>
    /// <returns>True if the parameter is valid; otherwise, false.</returns>
    bool IsValid();

    /// <summary>
    /// Indicates if the parameter is a variable.
    /// </summary>
    bool IsVariable();

    /// <summary>
    /// Indicates if the parameter is a value.
    /// </summary>
    /// <returns></returns>
    bool IsValue() => !IsVariable();

    /// <summary>
    /// Get the parameter type
    /// </summary>
    /// <returns></returns>
    IParameterType GetParameterType();
}

/// <summary>
/// Represents the type of a parameter value.
/// </summary>
public enum ParameterValueType
{
    /// <summary>
    /// Represents a parameter with a specific value.
    /// </summary>
    Value,

    /// <summary>
    /// Represents a parameter wich is a variable.
    /// </summary>
    Variable,
}
