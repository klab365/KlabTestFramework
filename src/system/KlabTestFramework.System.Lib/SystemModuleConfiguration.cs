using System;
using KlabTestFramework.System.Lib.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace KlabTestFramework.System.Lib;

public class SystemModuleConfiguration
{
    public Type ComponentRepositoryType { get; set; } = typeof(ComponentTomlRepository);
    public ServiceLifetime ComponentRepositoryLifetime { get; set; } = ServiceLifetime.Transient;
}
