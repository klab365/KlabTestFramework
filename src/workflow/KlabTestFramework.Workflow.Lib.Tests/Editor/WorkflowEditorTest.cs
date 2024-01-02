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
    public static ServiceProvider ServiceProvider { get; } = GetServiceProvider();

    private static ServiceProvider GetServiceProvider()
    {
        IServiceCollection services = new ServiceCollection();
        services.UseWorkflowLib(config => config.AddStepType<MockStep, MockStepHandler>());
        services.AddTransient<WorkflowEditor>();

        return services.BuildServiceProvider();
    }

    private readonly WorkflowEditor _sut;

    public WorkflowEditorTests()
    {
        _sut = ServiceProvider.GetRequiredService<WorkflowEditor>();
    }

    [Fact]
    public async Task CreateNewWorkflowShouldClearStepsAndMetaDataCallbacks()
    {
        // Arrange
        // Act
        _sut.CreateNewWorkflow();
        _sut.ConfigureMetadata(m => m.Description = "test");
        Result<Specifications.Workflow> workflowResult = await _sut.BuildWorkflowAsync();

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
        _sut.CreateNewWorkflow();
        _sut.AddStep<MockStep>();

        // Assert
        Result<Specifications.Workflow> workflowResult = await _sut.BuildWorkflowAsync();
        Specifications.Workflow workflow = workflowResult.Value!;
        workflow.Metadata.CreatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(2000));
        workflow.Metadata.UpdatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(2000));
        workflow.Steps.Should().HaveCount(1);
        workflow.Steps[0].Step.Should().BeOfType<MockStep>();
    }

    [Fact]
    public async Task BuildWorkflow_ShouldReturnTwoDifferentWorkflows_If_Called_Twice()
    {
        _sut.CreateNewWorkflow();
        _sut.AddStep<MockStep>();

        Result<Specifications.Workflow> workflow1Result = await _sut.BuildWorkflowAsync();
        Specifications.Workflow workflow1 = workflow1Result.Value!;
        Result<Specifications.Workflow> workflow2Result = await _sut.BuildWorkflowAsync();
        Specifications.Workflow workflow2 = workflow2Result.Value!;

        workflow1.Should().NotBeSameAs(workflow2);
        workflow1.Steps.Should().NotBeSameAs(workflow2.Steps);
        workflow1.Variables.Should().NotBeSameAs(workflow2.Variables);
    }

    [Fact]
    public async Task SaveWorkflowAsync_Should_Save_Expected_Data()
    {
        string dir = AppDomain.CurrentDomain.BaseDirectory;
        string path = Path.Combine(dir, "test.json");

        _sut.CreateNewWorkflow();
        _sut.AddStep<MockStep>(p => p.Counter.Content.SetValue(1));
        _sut.AddVariable<IntParameter>("test", "mV", VariableType.Constant, p => p.SetValue(1));

        Result<Specifications.Workflow> workflowResult = await _sut.BuildWorkflowAsync();
        Specifications.Workflow workflow = workflowResult.Value!;
        Result result = await _sut.SaveWorkflowAsync(path, workflow);
        result.IsSuccess.Should().BeTrue();

        Result<Specifications.Workflow> readWorkflow = await _sut.LoadWorkflowFromFileAsync(path)!;
        readWorkflow.IsSuccess.Should().BeTrue();
        workflow.ToData().Should().BeEquivalentTo(readWorkflow.Value!.ToData());
    }

    [Fact]
    public async Task CheckHasWorkflowErrors_Should_Be_True_If_Worfklow_NotValid()
    {
        _sut.CreateNewWorkflow();
        _sut.AddStep<MockStep>(p => p.Counter.Content.SetValue(-1));

        Result<Specifications.Workflow> workflowResult = await _sut.BuildWorkflowAsync();
        Result result = await _sut.CheckWorkflowHasErrorsAsync(workflowResult.Value!);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task CheckHasWorkflowErrors_Should_Be_True_If_Worfklow_Valid()
    {
        _sut.CreateNewWorkflow();
        _sut.AddStep<MockStep>(p => p.Counter.Content.SetValue(1));

        Result<Specifications.Workflow> workflowResult = await _sut.BuildWorkflowAsync();
        Result result = await _sut.CheckWorkflowHasErrorsAsync(workflowResult.Value!);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task EditWorkflow_Should_UpdateTimestamp_And_Build_Different_Workflow()
    {
        _sut.CreateNewWorkflow();
        _sut.AddStep<MockStep>(p => p.Counter.Content.SetValue(1));
        Result<Specifications.Workflow> workflowResult = await _sut.BuildWorkflowAsync();
        Specifications.Workflow workflow = workflowResult.Value!;
        DateTime firstCreatedDate = workflow.Metadata.CreatedAt;

        _sut.EditWorkflow(workflow);
        _sut.AddStep<MockStep>(p => p.Counter.Content.SetValue(2));
        Result<Specifications.Workflow> workflowResult2 = await _sut.BuildWorkflowAsync();
        Specifications.Workflow workflow2 = workflowResult2.Value!;

        workflow.Metadata.UpdatedAt.Should().BeBefore(workflow2.Metadata.UpdatedAt);
        workflow2.Metadata.CreatedAt.Should().Be(firstCreatedDate);
        workflow.Steps.Should().NotBeSameAs(workflow2.Steps);
    }
}
