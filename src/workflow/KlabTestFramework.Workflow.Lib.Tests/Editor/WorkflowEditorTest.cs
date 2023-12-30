using FluentAssertions;
using Klab.Toolkit.Results;
using KlabTestFramework.Workflow.Lib.Editor;
using Moq;

namespace KlabTestFramework.Workflow.Lib.Tests.Editor;

public class WorkflowEditorTests
{
    [Fact]
    public void CreateNewWorkflowShouldClearStepsAndMetaDataCallbacks()
    {
        // Arrange
        Mock<IWorkflowRepository> repositoryMock = new();
        Mock<IStepFactory> stepFactoryMock = new();
        WorkflowEditor editor = new(repositoryMock.Object, stepFactoryMock.Object);

        // Act
        editor.CreateNewWorkflow();
        editor.ConfigureMetadata(m => m.Description = "test");
        Result<Specifications.Workflow> workflowResult = editor.BuildWorkflow();
        if (workflowResult.IsFailure)
        {
            Assert.Fail("Workflow should be created successfully");
        }

        Specifications.Workflow workflow = workflowResult.Value!;
        workflow.Metadata.Description.Should().Be("test");
        workflow.Steps.Should().BeEmpty();
    }

    [Fact]
    public void AddStepShouldCreateStepAndAddToStepsList()
    {
        // Arrange
        Mock<IWorkflowRepository> repositoryMock = new();
        Mock<IStepFactory> stepFactoryMock = new();
        stepFactoryMock.Setup(m => m.CreateStep<MockStep>()).Returns(new MockStep());
        WorkflowEditor editor = new(repositoryMock.Object, stepFactoryMock.Object);

        // Act
        editor.CreateNewWorkflow();
        editor.AddStep<MockStep>();

        // Assert
        Specifications.Workflow workflow = editor.BuildWorkflow().Value!;
        workflow.Steps.Should().HaveCount(1);
        workflow.Steps[0].Step.Should().BeOfType<MockStep>();
    }
}
