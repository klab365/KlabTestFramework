using System;
using FluentAssertions;
using Moq;

namespace KlabTestFramework.Workflow.Lib.Specifications.Tests;

public class WorkflowTests
{
    [Fact]
    public void WorkflowShouldAddStepsAndVariables()
    {
        // Arrange
        StepContainer[] steps =
        [
            new StepContainer(new Mock<IStep>().Object),
            new StepContainer(new Mock<IStep>().Object)
        ];

        IVariable[] variables =
        [
            new Mock<IVariable>().Object,
            new Mock<IVariable>().Object
        ];

        // Act
        Workflow workflow = new(steps, variables);

        // Assert
        workflow.Steps.Should().HaveCount(2);
        workflow.Variables.Should().HaveCount(2);
    }

    [Fact]
    public void WorkflowShouldInitializeMetadata()
    {
        // Arrange
        // Act
        Workflow workflow = new(Array.Empty<StepContainer>(), Array.Empty<IVariable>());

        // Assert
        workflow.Metadata.Should().NotBeNull();
    }
}
