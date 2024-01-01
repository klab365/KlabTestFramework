namespace KlabTestFramework.Workflow.Lib.Specifications;

/// <summary>
/// Represents a parameter in the workflow specification.
/// </summary>
public interface IParameter
{
    /// <summary>
    /// Gets the name of the parameter.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the unit of the parameter.
    /// </summary>
    string Unit { get; }

    /// <summary>
    /// Gets or sets the variable name of the parameter.
    /// </summary>
    string VariableName { get; }

    /// <summary>
    /// Gets a value indicating what type of parameter it is.
    ParameterValueType ParameterType { get; }

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
    /// Get the data representation of the parameter.
    /// </summary>
    /// <returns></returns>
    ParameterData ToData();

    /// <summary>
    /// Fill the parameter with data.
    /// </summary>
    void FromData(ParameterData data);
}
