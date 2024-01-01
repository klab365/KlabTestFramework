namespace KlabTestFramework.Workflow.Lib.Specifications;

public class VariableData
{
    public string Name { get; set; } = string.Empty;

    public string Unit { get; set; } = string.Empty;

    public VariableType VariableType { get; set; }

    public string DataType { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;
}

public enum VariableType
{
    Constant,
    Argument,
}
