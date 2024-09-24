using System.Collections.Generic;

namespace KlabTestFramework.System.Lib.Specifications;

/// <summary>
/// Represents the data of a component.
/// </summary>
internal sealed record ComponentData
{
    public string Id { get; set; } = string.Empty;

    public bool IsEnabled { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string ImagePath { get; set; } = string.Empty;

    public Dictionary<string, string> Parameters { get; set; } = new();

    public List<ComponentData> Children { get; set; } = new();
}
