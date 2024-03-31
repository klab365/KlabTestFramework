using FluentAssertions;

namespace KlabTestFramework.Workflow.Lib.Specifications.Tests;

public class StepIdTests
{
    [Fact]
    public void AddRoute_Should_AddStepId()
    {
        StepId sut = StepId.Create("step-id");

        sut.AddRoute("route-id");

        sut.TotalId.Should().Be("/route-id/step-id");
    }
}
