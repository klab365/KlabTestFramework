using System.Collections.Generic;

namespace KlabTestFramework.Workflow.Lib.Specifications;

public class StepData
{
    public string Type { get; set; } = string.Empty;
    public List<ParameterData>? Parameters { get; set; }
}
