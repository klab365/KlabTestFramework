using Klab.Toolkit.Results;

namespace KlabTestFramework.System.Lib;

public static class SystemErrors
{
    public static readonly InformativeError PathIsRequired = new("System", "Path is required.", "Check the path and try again.");
    public static InformativeError ComponentNotFound(string id) => new("System", $"Component {id} not found.", "Check the component and try again.");
    public static InformativeError ComponentNotEnabled(string id) => new("System", $"Component {id} is not enabled.", "Check the component and try again.");
    public static InformativeError ComponentHasError(string id) => new("System", $"Component {id} has an error.", "Check the component and try again.");
    public static InformativeError DuplicateComponentId(string id) => new("System", $"Duplicate component id {id}.", "Check the component id and try again.");
    public static InformativeError ChildrenNotMatch(string id) => new("System", $"Children of component {id} do not match.", "Check the children and try again.");
    public static readonly InformativeError ComponentTypeMismatch = new("System", "Component type mismatch.", "Check the component type and try again.");
    public static readonly InformativeError Cancled = new("System", "Operation was canceled.", "Check the operation and try again.");
    public static readonly InformativeError ParameterNotFound = new("System", "Parameter not found.", "Check the parameter and try again.");
}
