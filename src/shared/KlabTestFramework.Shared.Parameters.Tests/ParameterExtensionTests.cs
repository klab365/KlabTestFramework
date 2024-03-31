using FluentAssertions;
using KlabTestFramework.Shared.Parameters.Types;

namespace KlabTestFramework.Shared.Parameters.Tests;

public class ParameterExtensionTests
{
    [Fact]
    public void CloneTest()
    {
        // arrange
        IntParameter intParameter = new();
        intParameter.SetValue(1);

        // act
        IntParameter clonedIntParameter = intParameter.Clone();

        // assert
        intParameter.Should().NotBeSameAs(clonedIntParameter);
    }
}
