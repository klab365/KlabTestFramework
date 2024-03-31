using FluentAssertions;
using KlabTestFramework.Shared.Parameters;
using Moq;
using NSubstitute;

namespace KlabTestFramework.Workflow.Lib.Specifications.Parameters.Tests;

public class ParameterTests
{
    [Fact]
    public void ParameterShouldInitializeProperties()
    {
        // Arrange
        string name = "ParameterName";
        string unit = "ParameterUnit";
        IParameterType content = Substitute.For<IParameterType>();
        content.Name.Returns(name);
        content.Unit.Returns(unit);

        // Act
        Parameter<IParameterType> parameter = new(name, unit, content);

        // Assert
        parameter.Name.Should().Be(name);
        parameter.Unit.Should().Be(unit);
        parameter.Content.Should().Be(content);
        parameter.VariableName.Should().BeEmpty();
        parameter.ParameterType.Should().Be(ParameterValueType.Value);
    }

    [Fact]
    public void ChangetToVariableShouldSetParameterTypeAndVariableName()
    {
        // Arrange
        Parameter<IParameterType> parameter = new("ParameterName", "ParameterUnit", new Mock<IParameterType>().Object);
        string variableName = "VariableName";

        // Act
        parameter.ChangetToVariable(variableName);

        // Assert
        parameter.ParameterType.Should().Be(ParameterValueType.Variable);
        parameter.VariableName.Should().Be(variableName);
    }

    [Fact]
    public void ChangeToValueShouldSetParameterTypeAndClearVariableName()
    {
        // Arrange
        Parameter<IParameterType> parameter = new("ParameterName", "ParameterUnit", new Mock<IParameterType>().Object);
        parameter.ChangetToVariable("VariableName");

        // Act
        parameter.ChangeToValue();

        // Assert
        parameter.ParameterType.Should().Be(ParameterValueType.Value);
        parameter.VariableName.Should().BeEmpty();
    }

    [Fact]
    public void ContentAsStringShouldReturnVariableNameWhenParameterTypeIsVariable()
    {
        // Arrange
        string variableName = "VariableName";
        Parameter<IParameterType> parameter = new("ParameterName", "ParameterUnit", new Mock<IParameterType>().Object);
        parameter.ChangetToVariable(variableName);

        // Act
        string contentAsString = parameter.ContentAsString();

        // Assert
        contentAsString.Should().Be(variableName);
    }

    [Fact]
    public void ContentAsStringShouldReturnContentAsStringWhenParameterTypeIsValue()
    {
        // Arrange
        string contentAsString = "ContentAsString";
        Mock<IParameterType> content = new();
        content.Setup(c => c.AsString()).Returns(contentAsString);
        Parameter<IParameterType> parameter = new("ParameterName", "ParameterUnit", content.Object);

        // Act
        string result = parameter.ContentAsString();

        // Assert
        result.Should().Be(contentAsString);
    }

    [Fact]
    public void IsValidShouldReturnTrueWhenContentIsValid()
    {
        // Arrange
        Mock<IParameterType> content = new();
        content.Setup(c => c.IsValid()).Returns(true);
        Parameter<IParameterType> parameter = new("ParameterName", "ParameterUnit", content.Object);

        // Act
        bool isValid = parameter.IsValid();

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public void IsValidShouldReturnFalseWhenContentIsInvalid()
    {
        // Arrange
        Mock<IParameterType> content = new();
        content.Setup(c => c.IsValid()).Returns(false);
        Parameter<IParameterType> parameter = new("ParameterName", "ParameterUnit", content.Object);

        // Act
        bool isValid = parameter.IsValid();

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public void ToDataShouldReturnParameterDataWithCorrectValues()
    {
        // Arrange
        string name = "ParameterName";
        string unit = "ParameterUnit";
        string contentAsString = "ContentAsString";
        IParameterType content = Substitute.For<IParameterType>();
        content.Name.Returns(name);
        content.Unit.Returns(unit);
        content.AsString().Returns(contentAsString);
        Parameter<IParameterType> parameter = new(name, unit, content);

        // Act
        ParameterData data = parameter.ToData();

        // Assert
        data.Name.Should().Be(name);
        data.Type.Should().Be(ParameterValueType.Value);
        data.Value.Should().Be(contentAsString);
    }
}
