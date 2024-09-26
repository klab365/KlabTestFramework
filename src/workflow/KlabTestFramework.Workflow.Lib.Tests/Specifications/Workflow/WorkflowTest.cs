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
        Workflow workflow = new();
        workflow.Steps.AddRange(steps);
        workflow.Variables.AddRange(variables);

        // Assert
        workflow.Steps.Should().HaveCount(2);
        workflow.Variables.Should().HaveCount(2);
    }

    [Fact]
    public void WorkflowShouldInitializeMetadata()
    {
        // Arrange
        // Act
        Workflow workflow = new();

        // Assert
        workflow.Steps.Should().BeEmpty();
        workflow.Variables.Should().BeEmpty();
        workflow.Subworkflows.Should().BeEmpty();
    }
}
