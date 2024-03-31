using System.Collections.Generic;

namespace KlabTestFramework.System.Lib.Specifications;

public record ComponentData
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string ImagePath { get; set; } = string.Empty;

    public List<ParameterData> Parameters { get; set; } = new();

    public List<ComponentData>? Children { get; set; }
}

public record ParameterData
{
    public string Name { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

    public string? Unit { get; set; }
}
