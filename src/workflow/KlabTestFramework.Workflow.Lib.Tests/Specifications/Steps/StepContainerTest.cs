using System.Collections.Generic;
using FluentAssertions;
using Moq;

namespace KlabTestFramework.Workflow.Lib.Specifications.Steps.Tests;

public class StepContainerTests
{
    [Fact]
    public void StepContainerShouldInitializeProperties()
    {
        // Arrange
        IStep step = new Mock<IStep>().Object;

        // Act
        StepContainer stepContainer = new(step);

        // Assert
        stepContainer.Id.Should().NotBeEmpty();
        stepContainer.Step.Should().Be(step);
        stepContainer.StepType.Should().Be(step.GetType());
    }

    [Fact]
    public void FromDataShouldSetParametersFromStepData()
    {
        // Arrange
        IStep step = new Mock<IStep>().Object;
        StepData stepData = new()
        {
            Parameters =
            [
                new ParameterData { Name = "Parameter1", Type = ParameterValueType.Value, Value = "Value1" },
                new ParameterData { Name = "Parameter2", Type = ParameterValueType.Variable, Value = "Variable1" }
            ]
        };
        Mock<IParameter> parameter1 = new();
        parameter1.Setup(p => p.Name).Returns("Parameter1");
        Mock<IParameter> parameter2 = new();
        parameter2.Setup(p => p.Name).Returns("Parameter2");
        Mock<IStep> stepMock = new();
        stepMock.Setup(s => s.GetParameters()).Returns(new List<IParameter> { parameter1.Object, parameter2.Object });
        StepContainer stepContainer = new(stepMock.Object);

        // Act
        stepContainer.FromData(stepData);

        // Assert
        parameter1.Verify(p => p.FromData(It.IsAny<ParameterData>()), Times.Once);
        parameter2.Verify(p => p.FromData(It.IsAny<ParameterData>()), Times.Once);
    }

    [Fact]
    public void FromDataShouldNotSetParametersWhenStepDataParametersIsNull()
    {
        // Arrange
        IStep step = new Mock<IStep>().Object;
        StepData stepData = new() { Parameters = null };
        Mock<IStep> stepMock = new();
        stepMock.Setup(s => s.GetParameters()).Returns(new List<IParameter>());
        StepContainer stepContainer = new(stepMock.Object);

        // Act
        stepContainer.FromData(stepData);

        // Assert
        stepMock.Verify(s => s.GetParameters(), Times.Never);
    }

    [Fact]
    public void ToDataShouldReturnStepDataWithCorrectValues()
    {
        // Arrange
        IStep step = new Mock<IStep>().Object;
        Mock<IParameter> parameter1 = new();
        parameter1.Setup(p => p.ToData()).Returns(new ParameterData { Name = "Parameter1", Type = ParameterValueType.Value, Value = "Value1" });
        Mock<IParameter> parameter2 = new();
        parameter2.Setup(p => p.ToData()).Returns(new ParameterData { Name = "Parameter2", Type = ParameterValueType.Variable, Value = "Variable1" });
        Mock<IStep> stepMock = new();
        stepMock.Setup(s => s.GetParameters()).Returns(new List<IParameter> { parameter1.Object, parameter2.Object });
        StepContainer stepContainer = new(stepMock.Object);

        // Act
        StepData stepData = stepContainer.ToData();

        // Assert
        stepData.Type.Should().Be(step.GetType().Name);
        stepData.Parameters.Should().HaveCount(2);
        stepData.Parameters.Should().ContainSingle(p => p.Name == "Parameter1" && p.Type == ParameterValueType.Value && p.Value == "Value1");
        stepData.Parameters.Should().ContainSingle(p => p.Name == "Parameter2" && p.Type == ParameterValueType.Variable && p.Value == "Variable1");
    }
}
