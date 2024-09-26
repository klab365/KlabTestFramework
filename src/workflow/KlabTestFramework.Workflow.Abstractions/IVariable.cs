using KlabTestFramework.Shared.Parameters;

namespace KlabTestFramework.Workflow.Lib.Specifications;


/// <summary>
/// Represents a variable in the workflow.
/// </summary>
public interface IVariable
{
    /// <summary>
    /// Gets the name of the variable.
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// Gets or sets the unit of the variable.
    /// </summary>
    string Unit { get; set; }

    /// <summary>
    /// Gets or sets the type of the variable.
    /// </summary>
    VariableType VariableType { get; set; }

    /// <summary>
    /// Gets the data type of the variable.
    /// </summary>
    string DataType { get; }

    bool IsArgument => VariableType == VariableType.Argument;

    /// <summary>
    /// Initialize the variable with the specified parameter type.
    /// </summary>
    void Init(IParameterType parameterType);

    /// <summary>
    /// Update the value of the variable.
    /// </summary>
    /// <param name="value"></param>
    void UpdateValue(string value);

    /// <summary>
    /// Get the parameter type
    /// </summary>
    /// <returns></returns>
    IParameterType GetParameterType();
}

public enum VariableType
{
    Constant,
    Argument,
}
