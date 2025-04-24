using Klab.Toolkit.Results;

namespace KlabTestFramework.System.Lib;

public static class SystemErrors
{
    private const string Code = "System";
    public static readonly Error PathIsRequired = Error.Create(Code, "Path is required.", "Check the path and try again.");
    public static Error ComponentNotFound(string id) => Error.Create(Code, $"Component {id} not found.", "Check the component and try again.");
    public static Error ComponentNotEnabled(string id) => Error.Create(Code, $"Component {id} is not enabled.", "Check the component and try again.");
    public static Error ComponentHasError(string id) => Error.Create(Code, $"Component {id} has an error.", "Check the component and try again.");
    public static Error DuplicateComponentId(string id) => Error.Create(Code, $"Duplicate component id {id}.", "Check the component id and try again.");
    public static Error ChildrenNotMatch(string id) => Error.Create(Code, $"Children of component {id} do not match.", "Check the children and try again.");
    public static readonly Error ComponentTypeMismatch = Error.Create(Code, "Component type mismatch.", "Check the component type and try again.");
    public static readonly Error Cancled = Error.Create(Code, "Operation was canceled.", "Check the operation and try again.");
    public static readonly Error ParameterNotFound = Error.Create(Code, "Parameter not found.", "Check the parameter and try again.");
}
