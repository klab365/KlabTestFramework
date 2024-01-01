using System;

namespace KlabTestFramework.Workflow.Lib.Specifications;

public interface IVariable
{
    string Name { get; }

    string Unit { get; set; }

    VariableType VariableType { get; set; }

    Type DataType { get; }

    void FromData(VariableData data);

    VariableData ToData();
}
