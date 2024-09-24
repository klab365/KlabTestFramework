using Klab.Toolkit.Results;

namespace KlabTestFramework.System.Abstractions;

public static class SystemErrors
{
    public static InformativeError ComponentTypeNotFound => new(1.ToString(), "Component type not found");

    public static InformativeError ParameterNotFound => new(2.ToString(), "Parameter not found", "Check the parameter name and value");

    public static InformativeError NoComponentSpecifications => new(3.ToString(), "No component specifications found");

    public static InformativeError ChildrenNotMatch(string id) => new(4.ToString(), "Children count does not match", $"Check the children count of the parent configuration '{id}'");
}
