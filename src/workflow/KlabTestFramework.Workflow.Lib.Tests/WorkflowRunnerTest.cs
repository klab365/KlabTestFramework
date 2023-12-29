using System.Threading.Tasks;
using FluentAssertions;
using KlabTestFramework.Workflow.Lib.Contracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace KlabTestFramework.Workflow.Lib.Tests;

public class WorkflowRunnerTests
{
    private readonly WorkflowRunner _sut;
    private readonly Mock<ILogger<WorkflowRunner>> _loggerMock;
    private readonly Mock<IStepFactory> _stepFactoryMock;

    public WorkflowRunnerTests()
    {
        _stepFactoryMock = new();
        _loggerMock = new();
        _sut = new(_loggerMock.Object, _stepFactoryMock.Object);
    }

    [Fact]
    public async Task RunAsyncShouldCallExpectedSteps()
    {
        // Arrange
        int invocationCounter = 0;
        _stepFactoryMock.Setup(m => m.CreateStep<MockStep>()).Returns(new MockStep());
        Mock<StepHandlerWrapperBase> stepHandlerMock = new();
        _stepFactoryMock.Setup(m => m.CreateStepHandler(It.IsAny<IStep>())).Returns(stepHandlerMock.Object);
        Specifications.Workflow workflow = new(_stepFactoryMock.Object);
        workflow.AddStep<MockStep>();
        workflow.AddStep<MockStep>();
        _sut.StepStatusChanged += (_, _) => invocationCounter++;

        // Act
        await _sut.RunAsync(workflow);

        // Assert
        invocationCounter.Should().Be(4);
    }

    [Fact]
    public async Task RunAsyncShouldNotCallAnyStepIfNotAvailable()
    {
        // Arrange
        int invocationCounter = 0;
        Specifications.Workflow workflow = new(_stepFactoryMock.Object);
        _sut.StepStatusChanged += (_, _) => invocationCounter++;

        // Act
        await _sut.RunAsync(workflow);

        // Assert
        invocationCounter.Should().Be(0);
    }
}
