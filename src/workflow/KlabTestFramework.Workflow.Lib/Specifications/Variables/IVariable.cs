using System;

namespace KlabTestFramework.Workflow.Lib.Specifications;


/// <summary>
/// Represents a variable in the workflow.
/// </summary>
public interface IVariable
{
    /// <summary>
    /// Gets the name of the variable.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets or sets the unit of the variable.
    /// </summary>
    string Unit { get; }

    /// <summary>
    /// Gets or sets the type of the variable.
    /// </summary>
    VariableType VariableType { get; }

    /// <summary>
    /// Gets the data type of the variable.
    /// </summary>
    Type DataType { get; }

    /// <summary>
    /// Initialize the variable with the specified parameter type.
    /// </summary>
    void Init(IParameterType parameterType);

    /// <summary>
    /// Initializes the variable from the specified data.
    /// </summary>
    /// <param name="data">The data to initialize the variable from.</param>
    void FromData(VariableData data);

    /// <summary>
    /// Converts the variable to its data representation.
    /// </summary>
    /// <returns>The data representation of the variable.</returns>
    VariableData ToData();
}
