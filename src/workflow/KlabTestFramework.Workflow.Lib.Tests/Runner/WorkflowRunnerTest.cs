using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;

using KlabTestFramework.Workflow.Lib.Editor;
using KlabTestFramework.Workflow.Lib.Specifications;
using KlabTestFramework.Workflow.Lib.Tests;
using Microsoft.Extensions.Logging;
using Moq;

namespace KlabTestFramework.Workflow.Lib.Runner.Tests;

public class WorkflowRunnerTests
{
    private readonly WorkflowRunner _sut;
    private readonly Mock<ILogger<WorkflowRunner>> _loggerMock;
    private readonly Mock<IEnumerable<StepSpecification>> _stepSpecificationsMock;

    public WorkflowRunnerTests()
    {
        _stepSpecificationsMock = new();
        _loggerMock = new();
        _sut = new(_loggerMock.Object, _stepSpecificationsMock.Object);
    }

    [Fact]
    public async Task RunAsyncShouldCallExpectedSteps()
    {
        // Arrange
        int invocationCounter = 0;
        Mock<IStepHandler<MockStep>> stepHandlerMock = new();
        Mock<IServiceProvider> serviceProviderMock = new();
        serviceProviderMock.Setup(s => s.GetService(typeof(IStepHandler<MockStep>))).Returns(stepHandlerMock.Object);
        serviceProviderMock.Setup(s => s.GetService(typeof(StepHandlerWrapper<MockStep>))).Returns(new StepHandlerWrapper<MockStep>(serviceProviderMock.Object));
        StepSpecification stepSpecification = StepSpecification.Create(typeof(MockStep), () => new MockStep(), () => new StepHandlerWrapper<MockStep>(serviceProviderMock.Object));
        _stepSpecificationsMock.Setup(s => s.GetEnumerator()).Returns(new List<StepSpecification> { stepSpecification }.GetEnumerator());
        Specifications.Workflow workflow = new([new MockStep(), new MockStep()], Array.Empty<IVariable>());
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
        Specifications.Workflow workflow = new(Array.Empty<IStep>(), Array.Empty<IVariable>());
        _sut.StepStatusChanged += (_, _) => invocationCounter++;

        // Act
        await _sut.RunAsync(workflow);

        // Assert
        invocationCounter.Should().Be(0);
    }
}
