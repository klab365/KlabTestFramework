namespace KlabTestFramework.Workflow.Lib.Specifications;

/// <summary>
/// Data class for a parameter
/// </summary>
public class ParameterData
{
    /// <summary>
    /// Name of the parameter
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Tyoe of the parameter (will be used to determine how to parse the value)
    /// </summary>
    public ParameterValueType Type { get; set; } = ParameterValueType.Value;

    /// <summary>
    /// Value in string format
    /// </summary>
    public string Value { get; set; } = string.Empty;
}

