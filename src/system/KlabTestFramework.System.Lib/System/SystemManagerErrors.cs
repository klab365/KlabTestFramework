using Klab.Toolkit.Results;

namespace KlabTestFramework.System.Lib.System;

public static class SystemManagerErrors
{
    public static readonly Error PathIsRequired = new(1, "Path is required.", "Check the path and try again.");
    public static readonly Error ComponentNotFound = new(2, "Component not found.", "Check the component and try again.");
    public static readonly Error ComponentTypeMismatch = new(3, "Component type mismatch.", "Check the component type and try again.");
    public static readonly Error Cancled = new(4, "Operation was canceled.", "Check the operation and try again.");
}
