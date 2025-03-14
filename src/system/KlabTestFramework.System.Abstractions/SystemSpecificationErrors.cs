using Klab.Toolkit.Results;

namespace KlabTestFramework.System.Abstractions;

public static class SystemErrors
{
    public static Error ComponentTypeNotFound => Error.Create("System", "Component type not found");

    public static Error ParameterNotFound => Error.Create("System", "Parameter not found", "Check the parameter name and value");

    public static Error NoComponentSpecifications => Error.Create("System", "No component specifications found");

    public static Error ChildrenNotMatch(string id) => Error.Create("System", "Children count does not match", $"Check the children count of the parent configuration '{id}'");
}
