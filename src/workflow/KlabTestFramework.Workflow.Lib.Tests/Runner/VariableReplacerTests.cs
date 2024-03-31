using System;
using System.Threading.Tasks;
using FluentAssertions;
using Klab.Toolkit.Results;
using KlabTestFramework.Shared.Parameters.Types;
using KlabTestFramework.Workflow.Lib.BuiltIn;
using KlabTestFramework.Workflow.Lib.Editor;
using KlabTestFramework.Workflow.Lib.Specifications;
using KlabTestFramework.Workflow.Lib.Tests;
using Microsoft.Extensions.DependencyInjection;

namespace KlabTestFramework.Workflow.Lib.Runner.Tests;

public class VariableReplacerTests
{
    [Fact]
    public async Task ReplaceVariablesShouldWorkAsExpected()
    {
        ServiceProvider serviceProvider = GetServiceProvider();
        VariableReplacer sut = serviceProvider.GetRequiredService<VariableReplacer>();
        IWorkflowEditor workflowEditor = serviceProvider.GetRequiredService<IWorkflowEditor>();
        workflowEditor.CreateNewWorkflow();
        workflowEditor.ConfigureMetadata(m => m.Description = "My first subworkflow");
        workflowEditor.AddVariable<IntParameter>("myVariable", "sec", VariableType.Argument, p => p.SetValue(5));
        MockStep mockStep = workflowEditor.AddStepToLastPosition<MockStep>(s => s.Counter.ChangetToVariable("myVariable"));
        Result<IWorkflow> workflow = await workflowEditor.BuildWorkflowAsync();

        await sut.ReplaceVariablesWithTheParametersAsync(workflow.Value!);

        mockStep.Counter.Content.Value.Should().Be(5);
    }

    [Fact]
    public async Task ReplaceVariablesShouldWorkAsExpected_WithSubworkflow()
    {
        ServiceProvider serviceProvider = GetServiceProvider();
        VariableReplacer sut = serviceProvider.GetRequiredService<VariableReplacer>();
        IWorkflowEditor workflowEditor = serviceProvider.GetRequiredService<IWorkflowEditor>();
        workflowEditor.CreateNewWorkflow();
        workflowEditor.ConfigureMetadata(m => m.Description = "My first subworkflow");
        workflowEditor.IncludeSubworkflow("sub1", await CreateSubworkflow1Async(serviceProvider));
        SubworkflowStep subStep = workflowEditor.AddStepToLastPosition<SubworkflowStep>(s =>
        {
            s.SelectSubworkflow("sub1");
            s.ReplaceArgumentValue("myVariable", "10");
        });
        Result<IWorkflow> workflow = await workflowEditor.BuildWorkflowAsync();

        await sut.ReplaceVariablesWithTheParametersAsync(workflow.Value!);

        MockStep mockStep = subStep.Children[0] as MockStep ?? throw new InvalidOperationException();
        mockStep.Counter.Content.Value.Should().Be(10);
    }

    [Fact]
    public async Task ReplaceVariablesShouldWorkAsExpected_WithTwoSubworkflowInvocations()
    {
        ServiceProvider serviceProvider = GetServiceProvider();
        VariableReplacer sut = serviceProvider.GetRequiredService<VariableReplacer>();
        IWorkflowEditor workflowEditor = serviceProvider.GetRequiredService<IWorkflowEditor>();
        workflowEditor.CreateNewWorkflow();
        workflowEditor.ConfigureMetadata(m => m.Description = "My first subworkflow");
        workflowEditor.IncludeSubworkflow("sub1", await CreateSubworkflow1Async(serviceProvider));
        SubworkflowStep subStep1 = workflowEditor.AddStepToLastPosition<SubworkflowStep>(s =>
        {
            s.SelectSubworkflow("sub1");
            s.ReplaceArgumentValue("myVariable", "10");
        });
        SubworkflowStep subStep2 = workflowEditor.AddStepToLastPosition<SubworkflowStep>(s =>
        {
            s.SelectSubworkflow("sub1");
            s.ReplaceArgumentValue("myVariable", "20");
        });
        Result<IWorkflow> workflow = await workflowEditor.BuildWorkflowAsync();

        await sut.ReplaceVariablesWithTheParametersAsync(workflow.Value!);

        MockStep mockStep1 = subStep1.Children[0] as MockStep ?? throw new InvalidOperationException();
        mockStep1.Counter.Content.Value.Should().Be(10);
        MockStep mockStep2 = subStep2.Children[0] as MockStep ?? throw new InvalidOperationException();
        mockStep2.Counter.Content.Value.Should().Be(20);
    }

    private static ServiceProvider GetServiceProvider(Action<IServiceCollection>? configure = null)
    {
        return ServiceProviderTestHelper.GetServiceProvider(services =>
        {
            services.AddTransient<VariableReplacer>();
            configure?.Invoke(services);
        });
    }

    private static Task<IWorkflow> CreateSubworkflow1Async(IServiceProvider services)
    {
        IWorkflowEditor workflowEditor = services.GetRequiredService<IWorkflowEditor>();
        workflowEditor.CreateNewWorkflow();
        workflowEditor.ConfigureMetadata(m => m.Description = "My first subworkflow");
        workflowEditor.AddVariable<IntParameter>("myVariable", "sec", VariableType.Argument, p => p.SetValue(2));
        workflowEditor.AddStepToLastPosition<MockStep>(s => s.Counter.ChangetToVariable("myVariable"));
        Result<IWorkflow> workflow = workflowEditor.BuildWorkflowAsync().Result;
        return Task.FromResult(workflow.Value!);
    }
}
