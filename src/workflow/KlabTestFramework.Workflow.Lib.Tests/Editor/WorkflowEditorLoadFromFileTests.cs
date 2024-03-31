using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Klab.Toolkit.Results;
using KlabTestFramework.Workflow.Lib.BuiltIn;
using KlabTestFramework.Workflow.Lib.Specifications;
using KlabTestFramework.Workflow.Lib.Tests;
using Microsoft.Extensions.DependencyInjection;

namespace KlabTestFramework.Workflow.Lib.Editor.Tests;

public class WorkflowEditorLoadFromFileTests
{
    [Fact]
    public async Task LoadWorkflowFromFileAsyncTest()
    {
        ServiceProvider serviceProvider = GetServiceProvider();
        WorkflowEditor sut = serviceProvider.GetRequiredService<WorkflowEditor>();

        string path = GetTestFilePath();
        Result resultReadWorkflow = await sut.LoadWorkflowFromFileAsync(path);
        resultReadWorkflow.IsFailure.Should().BeFalse();
        Result<IWorkflow> builtReadWorkflow = await sut.BuildWorkflowAsync();
        builtReadWorkflow.IsFailure.Should().BeFalse();

        IWorkflow readWorkflow = builtReadWorkflow.Value!;
        readWorkflow.Steps.Should().HaveCount(2);
        SubworkflowStep subworkflow1 = (SubworkflowStep)readWorkflow.Steps[0];
        subworkflow1.Arguments.Should().HaveCount(1);
        subworkflow1.Arguments[0].Name.Should().Be("myVariable");
        subworkflow1.Arguments[0].ContentAsString().Should().Be("00:00:10");

        var subworkflow2 = (SubworkflowStep)readWorkflow.Steps[1];
        subworkflow2.Arguments.Should().HaveCount(1);
        subworkflow2.Arguments[0].Name.Should().Be("myVariable");
        subworkflow2.Arguments[0].ContentAsString().Should().Be("00:00:06");
    }

    private static string GetTestFilePath()
    {
        return Path.Combine(Directory.GetCurrentDirectory(), "Editor", "assets", "workflow.json");
    }

    private static ServiceProvider GetServiceProvider()
    {
        return ServiceProviderTestHelper.GetServiceProvider(services =>
        {
            services.AddTransient<WorkflowEditor>();
        });
    }
}
