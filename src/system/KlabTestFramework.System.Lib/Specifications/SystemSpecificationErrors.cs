using Klab.Toolkit.Results;

namespace KlabTestFramework.System.Lib.Specifications;

public static class SystemSpecificationErrors
{
    public static Error ComponentTypeNotFound => new(1, "Component type not found");

    public static Error ParameterNotFound => new(2, "Parameter not found", "Check the parameter name and value");
}
