using System;
using System.Collections.Generic;
using FluentAssertions;
using KlabTestFramework.Workflow.Lib.Contracts;
using KlabTestFramework.Workflow.Lib.Specifications.Steps;
using Moq;

namespace KlabTestFramework.Workflow.Lib.Specifications.Tests;

public class WorkflowTests
{
    private readonly Workflow _sut;
    private readonly Mock<IStepFactory> _stepFactoryMock;

    public WorkflowTests()
    {
        _stepFactoryMock = new();
        _sut = new(_stepFactoryMock.Object);
    }

    [Fact]
    public void AddStepShouldAddStepToWorkflow()
    {
        // Arrange
        _stepFactoryMock.Setup(m => m.CreateStep<MockStep>()).Returns(new MockStep());

        // Act
        _sut.AddStep<MockStep>();

        // Assert
        _sut.Steps.Should().ContainSingle().Which.Step.Should().BeOfType<MockStep>();
    }

    [Fact]
    public void AddStepWithConfigureCallbackShouldAddStepToWorkflowAndInvokeCallback()
    {
        // Arrange
        MockStep step = new();
        bool configureCallbackInvoked = false;
        _stepFactoryMock.Setup(m => m.CreateStep<MockStep>()).Returns(step);

        // Act
        _sut.AddStep<MockStep>(s =>
        {
            configureCallbackInvoked = true;
            s.Should().Be(step);
        });

        // Assert
        configureCallbackInvoked.Should().BeTrue();
    }

    [Fact]
    public void DescriptionShouldBeEmptyByDefault()
    {
        // Assert
        _sut.Description.Should().BeEmpty();
    }

    [Fact]
    public void CreatedAtShouldBeSetToUtcNow()
    {
        // Assert
        _sut.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, precision: TimeSpan.FromSeconds(1));
    }
}

// Mock implementation of IStep for testing purposes
internal sealed class MockStep : IStep
{
    // Implement the required members of IStep interface
    public IEnumerable<IParameter> GetParameters()
    {
        yield break;
    }
}
