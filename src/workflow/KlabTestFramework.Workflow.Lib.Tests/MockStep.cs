using System;
using System.Collections.Generic;
using KlabTestFramework.Workflow.Lib.Specifications;


namespace KlabTestFramework.Workflow.Lib.Tests;

public class MockStep : IStep
{
    public IEnumerable<ParameterContainer> GetParameters()
    {
        throw new NotImplementedException();
    }

    public void Init(IEnumerable<ParameterData> parameterData)
    {
        throw new NotImplementedException();
    }
}
