using System.Diagnostics;
using System.Threading.Tasks;
using FluentAssertions;
using Klab.Toolkit.Results;
using KlabTestFramework.Workflow.Lib.Editor;
using KlabTestFramework.Workflow.Lib.Specifications;
using KlabTestFramework.Workflow.Lib.Tests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace KlabTestFramework.Workflow.Lib.Runner.Tests;

public class WorkflowRunnerBenchmarkTests
{
    private readonly ITestOutputHelper _outputHelper;

    public WorkflowRunnerBenchmarkTests(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task Run_Steps_Should_Be_Under_1Seconds_If_Step_Do_Nothing()
    {
        // Arrange
        ServiceProvider serviceProvider = GetServiceProvider();
        WorkflowRunner sut = serviceProvider.GetRequiredService<WorkflowRunner>();
        IWorkflowEditor editor = serviceProvider.GetRequiredService<IWorkflowEditor>();
        editor.CreateNewWorkflow();
        for (int i = 0; i < 1000; i++)
        {
            editor.AddStepToLastPosition<MockStep>();
        }
        Result<IWorkflow> res = await editor.BuildWorkflowAsync();
        IWorkflow workflow = res.Value!;

        // Act
        Stopwatch stopwatch = Stopwatch.StartNew();
        IWorkflowContext context = serviceProvider.GetRequiredService<IWorkflowContext>();
        await sut.RunAsync(workflow, context);
        stopwatch.Stop();

        // Assert
        _outputHelper.WriteLine($"Elapsed time: {stopwatch.Elapsed.TotalMilliseconds} ms");
        stopwatch.Elapsed.TotalMilliseconds.Should().BeLessThan(1000);
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
