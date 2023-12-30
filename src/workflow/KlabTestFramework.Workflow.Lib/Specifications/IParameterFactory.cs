using System;

namespace KlabTestFramework.Workflow.Lib.Specifications;

/// <summary>
/// Represents a factory for creating parameters used in the workflow.
/// </summary>
public interface IParameterFactory
{
    /// <summary>
    /// Creates a single value parameter with the specified display name, unit, initial value, and validation callbacks.
    /// </summary>
    /// <typeparam name="TValue">The type of the parameter value.</typeparam>
    /// <param name="displayName">The display name of the parameter.</param>
    /// <param name="unit">The unit of measurement for the parameter.</param>
    /// <param name="defaultValue">The initial value of the parameter.</param>
    /// <param name="isValidCallbacks">The validation callbacks for the parameter value.</param>
    /// <returns>The created single value parameter.</returns>
    SingleValueParameter<TValue> CreateSingleValueParameter<TValue>(string displayName, string unit, TValue defaultValue, params Func<TValue, bool>[] isValidCallbacks);

    /// <summary>
    /// Creates a choices parameter with the specified display name, unit, and choices.
    /// </summary>
    /// <typeparam name="TValue">The type of the parameter value.</typeparam>
    /// <param name="displayName">The display name of the parameter.</param>
    /// <param name="unit">The unit of measurement for the parameter.</param>
    /// <param name="choices">The available choices for the parameter.</param>
    /// <returns>The created choices parameter.</returns>
    ChoicesParameter<TValue> CreateChoicesParameter<TValue>(string displayName, string unit, params TValue[] choices);
}
