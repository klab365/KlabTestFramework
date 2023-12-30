using System;
using FluentAssertions;

namespace KlabTestFramework.Workflow.Lib.Specifications.Parameters.ParameterTypes.Tests;

public class ChoicesParameterTests
{
    [Fact]
    public void IsValidShouldReturnFalseWhenValueIsNull()
    {
        // Arrange
        ChoicesParameter<int> parameter = new();

        // Act
        bool isValid = parameter.IsValid();

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public void IsValidShouldReturnTrueIfInitIsCalled()
    {
        // Arrange
        ChoicesParameter<int> parameter = new();
        parameter.Init("Test Parameter", "Unit", [1, 2, 3]);

        // Act
        bool isValid = parameter.IsValid();

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public void IsValidShouldReturnTrueWhenValueIsInChoices()
    {
        // Arrange
        ChoicesParameter<int> parameter = new();
        parameter.Init("Test Parameter", "Unit", [1, 2, 3]);
        parameter.SetValue(2);

        // Act
        bool isValid = parameter.IsValid();

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public void SetValueShouldSetNewValue()
    {
        // Arrange
        ChoicesParameter<int> parameter = new();
        parameter.Init("Test Parameter", "Unit", [1, 2, 3]);

        // Act
        parameter.SetValue(3);

        // Assert
        parameter.Value.Should().Be(3);
    }

    [Fact]
    public void SetValueShouldThrowArgumentExceptionWhenValueIsNotInChoices()
    {
        // Arrange
        ChoicesParameter<int> parameter = new();
        parameter.Init("Test Parameter", "Unit", [1, 2, 3]);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => parameter.SetValue(4));
    }
}
