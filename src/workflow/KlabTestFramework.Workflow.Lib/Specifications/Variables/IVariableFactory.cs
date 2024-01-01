namespace KlabTestFramework.Workflow.Lib.Specifications;

public interface IVariableFactory
{
    /// <summary>
    /// Create a variable from data
    /// </summary>
    IVariable CreateVariableFromData(VariableData data);

    /// <summary>
    /// Create a new variable by parameter type
    /// </summary>
    IVariable CreateNewVariableByType(VariableData data, IParameterType parameterType);
}
