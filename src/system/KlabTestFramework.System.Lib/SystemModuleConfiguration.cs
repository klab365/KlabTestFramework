using System.Collections.Generic;
using KlabTestFramework.System.Abstractions;

namespace KlabTestFramework.System.Lib;

public class SystemModuleConfiguration
{
    public string? ConfigurationPath { get; set; }

    public List<ISystemSubmodule> Submodules { get; } = new();
}
