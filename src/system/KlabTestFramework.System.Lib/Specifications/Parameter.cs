using System;
using KlabTestFramework.Shared.Parameters;

namespace KlabTestFramework.System.Lib.Specifications;

public interface IParameter
{
    string Name { get; set; }

    string? Unit { get; set; }

    string Value { get; set; }

    bool IsValid();
}

public interface IParameter<out TParameterType> : IParameter where TParameterType : IParameterType
{
    TParameterType Content { get; }
}

public class Parameter<TParameterType> : IParameter<TParameterType> where TParameterType : IParameterType
{
    public string Name { get; set; } = string.Empty;
    public string? Unit { get; set; }
    public string Value { get; set; } = string.Empty;
    public TParameterType Content { get; }

    public Parameter(TParameterType content)
    {
        Content = content;
    }

    public bool IsValid()
    {
        return Content.IsValid();
    }
}

public static class ParameterFactoryExtensions
{
    public static Parameter<TParameterType> CreateParameter<TParameterType>(this ParameterFactory parameterFactory, string name, params Action<TParameterType>[]? configureCallbacks) where TParameterType : IParameterType
    {
        TParameterType type = parameterFactory.CreateParameterType<TParameterType>();
        if (configureCallbacks != null)
        {
            foreach (Action<TParameterType> callback in configureCallbacks)
            {
                callback(type);
            }
        }

        return new Parameter<TParameterType>(type) { Name = name };
    }
}
