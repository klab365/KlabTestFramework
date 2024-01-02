using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Klab.Toolkit.Results;
using KlabTestFramework.Workflow.Lib.BuiltIn;
using KlabTestFramework.Workflow.Lib.Editor;
using KlabTestFramework.Workflow.Lib.Specifications;
using Microsoft.Extensions.DependencyInjection;

namespace KlabTestFramework.Workflow.Lib.Tests.Editor;

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
        Result<Specifications.Workflow> workflowResult = await sut.BuildWorkflowAsync();

        // Assert
        workflowResult.IsFailure.Should().BeFalse("Workflow should be created successfully");
        Specifications.Workflow workflow = workflowResult.Value!;
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
        sut.AddStep<MockStep>();

        // Assert
        Result<Specifications.Workflow> workflowResult = await sut.BuildWorkflowAsync();
        Specifications.Workflow workflow = workflowResult.Value!;
        workflow.Metadata.CreatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(2000));
        workflow.Metadata.UpdatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(2000));
        workflow.Steps.Should().HaveCount(1);
        workflow.Steps[0].Step.Should().BeOfType<MockStep>();
    }

    [Fact]
    public async Task BuildWorkflow_ShouldReturnTwoDifferentWorkflows_If_Called_Twice()
    {
        ServiceProvider serviceProvider = GetServiceProvider();
        WorkflowEditor sut = serviceProvider.GetRequiredService<WorkflowEditor>();
        sut.CreateNewWorkflow();
        sut.AddStep<MockStep>();

        Result<Specifications.Workflow> workflow1Result = await sut.BuildWorkflowAsync();
        Specifications.Workflow workflow1 = workflow1Result.Value!;
        Result<Specifications.Workflow> workflow2Result = await sut.BuildWorkflowAsync();
        Specifications.Workflow workflow2 = workflow2Result.Value!;

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
        sut.AddStep<MockStep>(p => p.Counter.Content.SetValue(1));
        sut.AddVariable<IntParameter>("test", "mV", VariableType.Constant, p => p.SetValue(1));

        Result<Specifications.Workflow> workflowResult = await sut.BuildWorkflowAsync();
        Specifications.Workflow workflow = workflowResult.Value!;
        Result result = await sut.SaveWorkflowAsync(path, workflow);
        result.IsSuccess.Should().BeTrue();

        Result<Specifications.Workflow> readWorkflow = await sut.LoadWorkflowFromFileAsync(path)!;
        readWorkflow.IsSuccess.Should().BeTrue();
        workflow.ToData().Should().BeEquivalentTo(readWorkflow.Value!.ToData());
    }

    [Fact]
    public async Task CheckHasWorkflowErrors_Should_Be_True_If_Worfklow_NotValid()
    {
        ServiceProvider serviceProvider = GetServiceProvider();
        WorkflowEditor sut = serviceProvider.GetRequiredService<WorkflowEditor>();
        sut.CreateNewWorkflow();
        sut.AddStep<MockStep>(p => p.Counter.Content.SetValue(-1));

        Result<Specifications.Workflow> workflowResult = await sut.BuildWorkflowAsync();
        Result result = await sut.CheckWorkflowHasErrorsAsync(workflowResult.Value!);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task CheckHasWorkflowErrors_Should_Be_True_If_Worfklow_Valid()
    {
        ServiceProvider serviceProvider = GetServiceProvider();
        WorkflowEditor sut = serviceProvider.GetRequiredService<WorkflowEditor>();
        sut.CreateNewWorkflow();
        sut.AddStep<MockStep>(p => p.Counter.Content.SetValue(1));

        Result<Specifications.Workflow> workflowResult = await sut.BuildWorkflowAsync();
        Result result = await sut.CheckWorkflowHasErrorsAsync(workflowResult.Value!);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task EditWorkflow_Should_UpdateTimestamp_And_Build_Different_Workflow()
    {
        ServiceProvider serviceProvider = GetServiceProvider();
        WorkflowEditor sut = serviceProvider.GetRequiredService<WorkflowEditor>();
        sut.CreateNewWorkflow();
        sut.AddStep<MockStep>(p => p.Counter.Content.SetValue(1));
        Result<Specifications.Workflow> workflowResult = await sut.BuildWorkflowAsync();
        Specifications.Workflow workflow = workflowResult.Value!;
        DateTime firstCreatedDate = workflow.Metadata.CreatedAt;

        sut.EditWorkflow(workflow);
        sut.AddStep<MockStep>(p => p.Counter.Content.SetValue(2));
        Result<Specifications.Workflow> workflowResult2 = await sut.BuildWorkflowAsync();
        Specifications.Workflow workflow2 = workflowResult2.Value!;

        workflow.Metadata.UpdatedAt.Should().BeBefore(workflow2.Metadata.UpdatedAt);
        workflow2.Metadata.CreatedAt.Should().Be(firstCreatedDate);
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
