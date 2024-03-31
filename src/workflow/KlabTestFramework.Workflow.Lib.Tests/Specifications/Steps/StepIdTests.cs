using FluentAssertions;

namespace KlabTestFramework.Workflow.Lib.Specifications.Tests;

public class StepIdTests
{
    [Fact]
    public void AddRoute_Should_AddStepId()
    {
        StepId sut = StepId.Create("step-id");

        sut.AddRoute("route-id");

        sut.Value.Should().Be("route-idstep-id");
    }
}
