using FluentAssertions;

namespace KlabTestFramework.Workflow.Lib.Specifications.Parameters.ParameterTypes.Tests;

public class SingleValueParameterTests
{

    [Fact]
    public void IsValidShouldReturnFalseWhenValueDoesNotSatisfyIsValidCallbacks()
    {
        // Arrange
        SingleValueParameter<int> parameter = new();
        parameter.Init("Test Parameter", "Unit", 10, [x => x > 11]);

        // Act
        bool isValid = parameter.IsValid();

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public void IsValidShouldReturnTrueWhenValueSatisfiesAllIsValidCallbacks()
    {
        // Arrange
        SingleValueParameter<int> parameter = new();
        parameter.Init("Test Parameter", "Unit", 10, [x => x > 0, x => x < 100]);

        // Act
        bool isValid = parameter.IsValid();

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public void SetValueShouldSetNewValue()
    {
        // Arrange
        SingleValueParameter<int> parameter = new();

        // Act
        parameter.SetValue(42);

        // Assert
        parameter.Value.Should().Be(42);
    }
}
