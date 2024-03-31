using FluentAssertions;
using KlabTestFramework.Shared.Parameters;
using KlabTestFramework.Shared.Parameters.Types;
using KlabTestFramework.Workflow.Lib.Tests;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NSubstitute;

namespace KlabTestFramework.Workflow.Lib.Specifications.Tests;

public class VariableTests
{
    [Fact]
    public void VariableShouldInitializeProperties()
    {
        // Arrange
        IParameterType mockParameter = Substitute.For<IParameterType>();
        mockParameter.TypeKey.Returns("IParameterType");

        // Act
        Variable<IParameterType> variable = new();
        variable.Init(mockParameter);

        // Assert
        variable.Name.Should().BeEmpty();
        variable.Unit.Should().BeEmpty();
        variable.VariableType.Should().Be(VariableType.Constant);
        variable.Parameter.Should().Be(mockParameter);
        variable.DataType.Should().Be("IParameterType");
    }

    [Fact]
    public void VariableShouldConvertToData()
    {
        // Arrange
        VariableType expectedVariableType = VariableType.Constant;
        string expectedValue = "ParameterValue";
        IParameterType mockParameter = Substitute.For<IParameterType>();
        mockParameter.AsString().Returns(expectedValue);
        mockParameter.TypeKey.Returns("IParameterType");

        Variable<IParameterType> variable = new();
        variable.Init(mockParameter);

        // Act
        VariableData data = variable.ToData();

        // Assert
        data.Name.Should().BeEmpty();
        data.Unit.Should().BeEmpty();
        data.VariableType.Should().Be(expectedVariableType);
        data.DataType.Should().Be("IParameterType");
        data.Value.Should().Be(expectedValue);
    }

    [Fact]
    public void VariableShouldConvertFromData()
    {
        // Arrange
        string expectedName = "VariableName";
        string expectedUnit = "VariableUnit";
        VariableType expectedVariableType = VariableType.Constant;
        string expectedValue = "ParameterValue";
        VariableData data = new()
        {
            Name = expectedName,
            Unit = expectedUnit,
            VariableType = expectedVariableType,
            DataType = typeof(IParameterType).Name,
            Value = expectedValue
        };
        IParameterType mockParameter = Mock.Of<IParameterType>();

        Variable<IParameterType> variable = new();
        variable.Init(mockParameter);

        // Act
        variable.FromData(data);

        // Assert
        variable.Name.Should().Be(expectedName);
        variable.Unit.Should().Be(expectedUnit);
        variable.VariableType.Should().Be(expectedVariableType);
        variable.Parameter.Should().Be(mockParameter);
        Mock.Get(mockParameter).Verify(p => p.FromString(expectedValue), Times.Once);
    }

    [Fact]
    public void VariableGenerateGenericParameterType()
    {
        ServiceProvider serviceProvider = ServiceProviderTestHelper.GetServiceProvider();
        ParameterFactory parameterFactory = serviceProvider.GetRequiredService<ParameterFactory>();
        SelectableParameter<StringParameter> parameter = parameterFactory.CreateParameterType<SelectableParameter<StringParameter>>();
        parameter.SetValue(new StringParameter());
        Variable<SelectableParameter<StringParameter>> variable = new();
        variable.Init(parameter);

        VariableData data = variable.ToData();

        data.DataType.Should().Be("SelectableParameter<StringParameter>");
    }
}
