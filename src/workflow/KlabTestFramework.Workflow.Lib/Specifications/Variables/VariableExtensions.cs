
namespace KlabTestFramework.Workflow.Lib.Specifications;

public static class VariableExtensions
{
    // <inheritdoc/>
    public static void FromData(this IVariable variable, VariableData data)
    {
        variable.Name = data.Name;
        variable.Unit = data.Unit;
        variable.VariableType = data.VariableType;
        variable.GetParameterType().FromString(data.Value ?? string.Empty);
    }

    /// <inheritdoc/>
    public static VariableData ToData(this IVariable variable)
    {
        return new()
        {
            Name = variable.Name,
            Unit = variable.Unit,
            VariableType = variable.VariableType,
            DataType = variable.DataType,
            Value = variable.GetParameterType().AsString() ?? string.Empty
        };
    }
}
