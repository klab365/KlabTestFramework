using System;
using System.Collections.Generic;
using KlabTestFramework.Workflow.Lib.Contracts;

namespace KlabTestFramework.Workflow.Lib.Tests;

public class MockStep : IStep
{
    public IEnumerable<ParameterData>? GetParameterData()
    {
        throw new NotImplementedException();
    }

    public void Init(IEnumerable<ParameterData> parameterData)
    {
        throw new NotImplementedException();
    }
}
