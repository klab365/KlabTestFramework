using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;

namespace KlabTestFramework.Workflow.Lib.Specifications.Tests;

public class WorkflowTests
{
    [Fact]
    public void WorkflowShouldAddStepsAndVariables()
    {
        // Arrange
        IStep[] steps =
        [
            new Mock<IStep>().Object,
            new Mock<IStep>().Object,
        ];

        IVariable[] variables =
        [
            new Mock<IVariable>().Object,
            new Mock<IVariable>().Object
        ];

        // Act
        Workflow workflow = new(steps, variables, new Dictionary<string, IWorkflow>());

        // Assert
        workflow.Steps.Should().HaveCount(2);
        workflow.Variables.Should().HaveCount(2);
    }

    [Fact]
    public void WorkflowShouldInitializeMetadata()
    {
        // Arrange
        // Act
        Workflow workflow = new(Array.Empty<IStep>(), Array.Empty<IVariable>(), new Dictionary<string, IWorkflow>());

        // Assert
        workflow.Metadata.Should().NotBeNull();
    }
}
