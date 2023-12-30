using System;
using System.Collections.Generic;

namespace KlabTestFramework.Workflow.Lib.Specifications;

public class WorkflowData
{
    public string Description { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<StepData> Steps { get; set; } = new();
}
