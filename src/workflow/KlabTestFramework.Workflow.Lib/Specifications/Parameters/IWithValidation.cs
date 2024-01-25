

using System;
using System.Collections.Generic;

namespace KlabTestFramework.Workflow.Lib.Specifications;

/// <summary>
/// Represents an interface for objects that support validation of a specific parameter type.
/// </summary>
/// <typeparam name="TType">The type of the parameter.</typeparam>
public interface IWithValidation<TType>
{
    /// <summary>
    /// Gets the list of validation callbacks for the parameter type.
    /// </summary>
    List<Func<TType, bool>> ValidaCallbacks { get; }

    /// <summary>
    /// Adds a validation callback for the parameter type.
    /// </summary>
    /// <param name="value">The validation callback to add.</param>
    void AddValiation(Func<TType, bool> value)
    {
        ValidaCallbacks.Add(value);
    }
}
