using System;

namespace KlabTestFramework.System.Abstractions;

public record ComponentSpecification(
    string Type,
    Func<IComponentConfig> CreateConfig,
    Func<IComponent> CreateComponent
);
