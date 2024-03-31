using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Klab.Toolkit.Results;
using KlabTestFramework.Shared.Parameters.Types;
using KlabTestFramework.Workflow.Lib.Specifications;
using KlabTestFramework.Workflow.Lib.Tests;
using Microsoft.Extensions.DependencyInjection;

namespace KlabTestFramework.Workflow.Lib.Editor.Tests;

public class WorkflowEditorTests
{
    [Fact]
    public async Task CreateNewWorkflowShouldClearStepsAndMetaDataCallbacks()
    {
        // Arrange
        // Act
        ServiceProvider serviceProvider = GetServiceProvider();
        WorkflowEditor sut = serviceProvider.GetRequiredService<WorkflowEditor>();
        sut.CreateNewWorkflow();
        sut.ConfigureMetadata(m => m.Description = "test");
        Result<IWorkflow> workflowResult = await sut.BuildWorkflowAsync();

        // Assert
        workflowResult.IsFailure.Should().BeFalse("Workflow should be created successfully");
        IWorkflow workflow = workflowResult.Value!;
        workflow.Metadata.Description.Should().Be("test");
        workflow.Steps.Should().BeEmpty();
    }

    [Fact]
    public async Task AddStepShouldCreateStepAndAddToStepsList()
    {
        // Arrange
        // Act
        ServiceProvider serviceProvider = GetServiceProvider();
        WorkflowEditor sut = serviceProvider.GetRequiredService<WorkflowEditor>();
        sut.CreateNewWorkflow();
        sut.AddStepToLastPosition<MockStep>();

        // Assert
        Result<IWorkflow> workflowResult = await sut.BuildWorkflowAsync();
        IWorkflow workflow = workflowResult.Value!;
        workflow.Steps.Should().HaveCount(1);
        workflow.Steps[0].Should().BeOfType<MockStep>();
    }

    [Fact]
    public async Task BuildWorkflow_ShouldReturnTwoDifferentWorkflows_If_Called_Twice()
    {
        ServiceProvider serviceProvider = GetServiceProvider();
        WorkflowEditor sut = serviceProvider.GetRequiredService<WorkflowEditor>();
        sut.CreateNewWorkflow();
        sut.AddStepToLastPosition<MockStep>();

        Result<IWorkflow> workflow1Result = await sut.BuildWorkflowAsync();
        IWorkflow workflow1 = workflow1Result.Value!;
        Result<IWorkflow> workflow2Result = await sut.BuildWorkflowAsync();
        IWorkflow workflow2 = workflow2Result.Value!;

        workflow1.Should().NotBeSameAs(workflow2);
        workflow1.Steps.Should().NotBeSameAs(workflow2.Steps);
        workflow1.Variables.Should().NotBeSameAs(workflow2.Variables);
    }

    [Fact]
    public async Task SaveWorkflowAsync_Should_Save_Expected_Data()
    {
        ServiceProvider serviceProvider = GetServiceProvider();
        WorkflowEditor sut = serviceProvider.GetRequiredService<WorkflowEditor>();
        string dir = AppDomain.CurrentDomain.BaseDirectory;
        string path = Path.Combine(dir, "test.json");

        sut.CreateNewWorkflow();
        sut.AddStepToLastPosition<MockStep>(p => p.Counter.Content.SetValue(1));
        sut.AddVariable<IntParameter>("test", "mV", VariableType.Constant, p => p.SetValue(1));

        Result result = await sut.SaveWorkflowAsync(path);
        result.IsSuccess.Should().BeTrue();

        Result resultLoadWorkflow = await sut.LoadWorkflowFromFileAsync(path);
        resultLoadWorkflow.IsSuccess.Should().BeTrue();
        Result<IWorkflow> resultBuildWorkflow = await sut.BuildWorkflowAsync();
        resultBuildWorkflow.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task EditWorkflow_Should_UpdateTimestamp_And_Build_Different_Workflow()
    {
        ServiceProvider serviceProvider = GetServiceProvider();
        WorkflowEditor sut = serviceProvider.GetRequiredService<WorkflowEditor>();
        sut.CreateNewWorkflow();
        sut.AddStepToLastPosition<MockStep>(p => p.Counter.Content.SetValue(1));
        Result<IWorkflow> workflowResult = await sut.BuildWorkflowAsync();
        IWorkflow workflow = workflowResult.Value!;

        sut.EditWorkflow(workflow);
        sut.AddStepToLastPosition<MockStep>(p => p.Counter.Content.SetValue(2));
        Result<IWorkflow> workflowResult2 = await sut.BuildWorkflowAsync();
        IWorkflow workflow2 = workflowResult2.Value!;

        workflow.Steps.Should().NotBeSameAs(workflow2.Steps);
    }

    private static ServiceProvider GetServiceProvider()
    {
        return ServiceProviderTestHelper.GetServiceProvider(services =>
        {
            services.AddTransient<WorkflowEditor>();
        });
    }
}
