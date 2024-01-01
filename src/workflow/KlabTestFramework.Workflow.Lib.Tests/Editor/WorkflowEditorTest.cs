using FluentAssertions;
using Klab.Toolkit.Results;
using KlabTestFramework.Workflow.Lib.Editor;
using KlabTestFramework.Workflow.Lib.Specifications;
using Moq;

namespace KlabTestFramework.Workflow.Lib.Tests.Editor;

public class WorkflowEditorTests
{
    private readonly Mock<IWorkflowRepository> _repositoryMock;
    private readonly Mock<IStepFactory> _stepFactoryMock;
    private readonly Mock<IParameterFactory> _parameterFactoryMock;
    private readonly Mock<IVariableFactory> _variableFactoryMock;
    private readonly WorkflowEditor _sut;

    public WorkflowEditorTests()
    {
        _repositoryMock = new();
        _stepFactoryMock = new();
        _parameterFactoryMock = new();
        _variableFactoryMock = new();
        _sut = new(_repositoryMock.Object, _stepFactoryMock.Object, _parameterFactoryMock.Object, _variableFactoryMock.Object);
    }

    [Fact]
    public void CreateNewWorkflowShouldClearStepsAndMetaDataCallbacks()
    {
        // Arrange
        // Act
        _sut.CreateNewWorkflow();
        _sut.ConfigureMetadata(m => m.Description = "test");
        Result<Specifications.Workflow> workflowResult = _sut.BuildWorkflow();
        if (workflowResult.IsFailure)
        {
            Assert.Fail("Workflow should be created successfully");
        }

        Specifications.Workflow workflow = workflowResult.Value!;
        workflow.Metadata.Description.Should().Be("test");
        workflow.Steps.Should().BeEmpty();
    }

    // [Fact]
    // public void AddStepShouldCreateStepAndAddToStepsList()
    // {
    //     // Arrange
    //     _stepFactoryMock.Setup(m => m.CreateStep<MockStep>()).Returns(new MockStep());

    //     // Act
    //     _sut.CreateNewWorkflow();
    //     _sut.AddStep<MockStep>();

    //     // Assert
    //     Specifications.Workflow workflow = _sut.BuildWorkflow().Value!;
    //     workflow.Steps.Should().HaveCount(1);
    //     workflow.Steps[0].Step.Should().BeOfType<MockStep>();
    // }
}
