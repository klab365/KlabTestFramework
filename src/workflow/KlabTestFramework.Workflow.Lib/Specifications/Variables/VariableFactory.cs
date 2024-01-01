using System;

namespace KlabTestFramework.Workflow.Lib.Specifications;

/// <summary>
/// Implementation of the <see cref="IVariableFactory"/> interface.
/// </summary>
public class VariableFactory : IVariableFactory
{
    private readonly IParameterFactory _parameterFactory;
    private readonly Func<IParameterType, IVariable> _variableFactory;

    public VariableFactory(IParameterFactory parameterFactory, Func<IParameterType, IVariable> variableFactory)
    {
        _parameterFactory = parameterFactory;
        _variableFactory = variableFactory;
    }

    public IVariable CreateNewVariableByType(VariableData data, IParameterType parameterType)
    {
        IVariable variable = _variableFactory.Invoke(parameterType);
        variable.Init(parameterType);
        variable.FromData(data);
        return variable;
    }

    /// <inheritdoc/>
    public IVariable CreateVariableFromData(VariableData data)
    {
        IParameterType parameterType = _parameterFactory.CreateParameterTypeByName(data.DataType);
        IVariable variable = CreateNewVariableByType(data, parameterType);
        return variable;
    }
}
