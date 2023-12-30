using System.Collections.Generic;
using KlabTestFramework.Workflow.Lib.Contracts;

namespace KlabTestFramework.Workflow.Lib.Specifications;

public class StepData
{
    public string Type { get; set; } = string.Empty;
    public List<ParameterData>? Parameters { get; set; }
}
