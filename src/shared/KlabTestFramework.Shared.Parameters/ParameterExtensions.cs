using System;

namespace KlabTestFramework.Shared.Parameters;

public static class ParameterExtensions
{
    /// <summary>
    /// Clones the parameter.
    /// </summary>
    public static TParameterType Clone<TParameterType>(this TParameterType parameter) where TParameterType : IParameterType
    {
        IParameterType? clone = (IParameterType?)Activator.CreateInstance(parameter.GetType());
        if (clone == null)
        {
            throw new InvalidOperationException("Could not create a clone of the parameter");
        }

        clone.FromString(parameter.AsString());
        return (TParameterType)clone;
    }
}
