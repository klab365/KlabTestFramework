using System;
using FluentAssertions;
using KlabTestFramework.Workflow.Lib.Contracts;

namespace KlabTestFramework.Workflow.Lib.Specifications.Tests;

public class WorkflowTests
{
    [Fact]
    public void DescriptionShouldBeEmptyByDefault()
    {
        Workflow sut = new(Array.Empty<IStep>());
        sut.Metadata.Description.Should().BeEmpty();
    }
}
