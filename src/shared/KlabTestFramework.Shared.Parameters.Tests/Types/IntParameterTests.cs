
using System;
using FluentAssertions;
using KlabTestFramework.Shared.Parameters.Tests;
using Microsoft.Extensions.DependencyInjection;

namespace KlabTestFramework.Shared.Parameters.Types.Tests;


public class IntParameterTests
{
    [Fact]
    public void CreationTest()
    {
        IServiceProvider serviceProvider = ServiceProviderTestHelper.CreateServiceProvider();
        ParameterFactory parameterFactory = serviceProvider.GetRequiredService<ParameterFactory>();
        IntParameter sut = parameterFactory.CreateParameterType<IntParameter>();
        sut.Name = "Test";
        sut.Unit = "unit";
        sut.SetValue(1);

        sut.Name.Should().Be("Test");
        sut.Unit.Should().Be("unit");
        sut.Value.Should().Be(1);
    }
}
