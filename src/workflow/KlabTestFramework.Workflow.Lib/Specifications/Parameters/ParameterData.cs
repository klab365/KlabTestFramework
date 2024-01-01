namespace KlabTestFramework.Workflow.Lib.Specifications;

public class ParameterData
{
    public string Name { get; set; } = string.Empty;
    public bool? IsVariable { get; set; }
    // public ParameterValueType? Type { get; set; }
    public string Value { get; set; } = string.Empty;
}
