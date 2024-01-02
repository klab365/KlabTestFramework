using System.Threading.Tasks;
using FluentAssertions;
using Klab.Toolkit.Results;
using KlabTestFramework.Workflow.Lib.Editor;
using KlabTestFramework.Workflow.Lib.Tests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace KlabTestFramework.Workflow.Lib.Runner.Tests;

public class WorkflowRunnerTests
{
    [Fact]
    public async Task RunAsyncShouldCallExpectedSteps()
    {
        // Arrange
        int invocationCounter = 0;
        ServiceProvider serviceProvider = GetServiceProvider();
        WorkflowRunner sut = serviceProvider.GetRequiredService<WorkflowRunner>();
        IWorkflowEditor editor = serviceProvider.GetRequiredService<IWorkflowEditor>();
        editor.CreateNewWorkflow();
        editor.AddStep<MockStep>();
        editor.AddStep<MockStep>();
        Result<Specifications.Workflow> res = await editor.BuildWorkflowAsync();
        Specifications.Workflow workflow = res.Value!;
        sut.StepStatusChanged += (_, _) => invocationCounter++;

        // Act
        await sut.RunAsync(workflow);

        // Assert
        invocationCounter.Should().Be(4);
    }

    [Fact]
    public async Task RunAsyncShouldNotCallAnyStepIfNotAvailable()
    {
        // Arrange
        int invocationCounter = 0;
        ServiceProvider serviceProvider = GetServiceProvider();
        WorkflowRunner sut = serviceProvider.GetRequiredService<WorkflowRunner>();
        IWorkflowEditor editor = serviceProvider.GetRequiredService<IWorkflowEditor>();
        editor.CreateNewWorkflow();
        Result<Specifications.Workflow> res = await editor.BuildWorkflowAsync();
        Specifications.Workflow workflow = res.Value!;

        // Act
        await sut.RunAsync(workflow);

        // Assert
        invocationCounter.Should().Be(0);
    }

    [Fact]
    public async Task RunAsync_Should_Not_HandleStep_If_NotValid()
    {
        int invocationCounter = 0;
        ServiceProvider serviceProvider = GetServiceProvider();
        WorkflowRunner sut = serviceProvider.GetRequiredService<WorkflowRunner>();
        IWorkflowEditor editor = serviceProvider.GetRequiredService<IWorkflowEditor>();
        editor.CreateNewWorkflow();
        editor.AddStep<MockStep>(p => p.Counter.Content.SetValue(-1));
        Result<Specifications.Workflow> res = await editor.BuildWorkflowAsync();
        Specifications.Workflow workflow = res.Value!;

        WorkflowResult resRun = await sut.RunAsync(workflow);

        resRun.IsSuccess.Should().BeFalse();
        invocationCounter.Should().Be(0);
    }

    private static ServiceProvider GetServiceProvider()
    {
        return ServiceProviderTestHelper.GetServiceProvider(services =>
        {
            services.AddTransient<WorkflowRunner>();
            services.AddTransient(_ => Mock.Of<ILogger<WorkflowRunner>>());
        });
    }
}
