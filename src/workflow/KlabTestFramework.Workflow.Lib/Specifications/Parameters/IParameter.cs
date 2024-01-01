namespace KlabTestFramework.Workflow.Lib.Specifications;

/// <summary>
/// Interface for a parameter
/// </summary>
public interface IParameter
{
    /// <summary>
    /// Gets the display name of the parameter.
    /// </summary>
    string DisplayName { get; }

    /// <summary>
    /// Gets the unit of the parameter.
    /// </summary>
    string Unit { get; }

    /// <summary>
    /// Gets the variable name of the parameter.
    /// </summary>
    string VariableName { get; set; }

    bool IsVariable { get; }

    void ChangetToVariable(string variableName);

    void ChangeToValue();

    string ContentAsString();

    string ToData();
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
    Variable
}
