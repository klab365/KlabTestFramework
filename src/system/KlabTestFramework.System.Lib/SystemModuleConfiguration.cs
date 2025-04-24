using System;
using System.Collections.Generic;
using KlabTestFramework.System.Abstractions;
using KlabTestFramework.System.Lib.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace KlabTestFramework.System.Lib;

public class SystemModuleConfiguration
{
    private readonly List<ComponentSpecification> _componentSpecifications = new();
    public IReadOnlyList<ComponentSpecification> ComponentSpecifications => _componentSpecifications;

    public Type ComponentRepositoryType { get; set; } = typeof(ComponentTomlRepository);
    public ServiceLifetime ComponentRepositoryLifetime { get; set; } = ServiceLifetime.Transient;

    public SystemModuleConfiguration RegisterComponent(ComponentSpecification specification)
    {
        _componentSpecifications.Add(specification);
        return this;
    }
}
