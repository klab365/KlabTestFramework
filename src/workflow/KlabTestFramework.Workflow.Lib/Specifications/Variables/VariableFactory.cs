using System;
using System.Collections.Generic;
using System.Linq;
using KlabTestFramework.Shared.Parameters;
using KlabTestFramework.Workflow.Lib.BuiltIn;

namespace KlabTestFramework.Workflow.Lib.Specifications;

public class VariableFactory
{
    private readonly ParameterFactory _parameterFactory;
    private readonly IEnumerable<VariableDependenySpecification> _variableSpecifications;

    public VariableFactory(ParameterFactory parameterFactory, IEnumerable<VariableDependenySpecification> variableSpecifications)
    {
        _parameterFactory = parameterFactory;
        _variableSpecifications = variableSpecifications;
    }

    /// <inheritdoc/>
    public static IVariable CreateNewVariableByType(VariableData data, IParameterType parameterType)
    {
        object? createdVariable = Activator.CreateInstance(typeof(Variable<>).MakeGenericType(parameterType.GetType()));
        if (createdVariable is not IVariable variable)
        {
            throw new InvalidOperationException($"Failed to create variable for type {parameterType.GetType().Name}");
        }

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

    public IVariableParameterReplaceHandler CreateVariableReplaceHandler(Type parameterType)
    {
        VariableDependenySpecification? variableDependeny = _variableSpecifications.SingleOrDefault(s => s.ParameterType == parameterType);
        if (variableDependeny is not null)
        {
            IVariableParameterReplaceHandler variableParameterReplaceHandler = variableDependeny.VariableReplaceFactory();
            return variableParameterReplaceHandler;
        }

        object? createdDefaultVariableReplaceHandler = Activator.CreateInstance(typeof(DefaultVariableParameterReplace<>).MakeGenericType(parameterType));
        if (createdDefaultVariableReplaceHandler is not IVariableParameterReplaceHandler defaultVariableReplaceHandler)
        {
            throw new InvalidOperationException($"Failed to create variable replace handler for type {parameterType.Name}");
        }
        return defaultVariableReplaceHandler;
    }
}

public record VariableDependenySpecification(
    Type ParameterType,
    Func<IVariableParameterReplaceHandler> VariableReplaceFactory);
