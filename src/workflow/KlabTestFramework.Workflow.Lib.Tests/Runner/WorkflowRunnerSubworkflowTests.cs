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
using Microsoft.Extensions.Logging;
using Moq;

namespace KlabTestFramework.Workflow.Lib.Runner.Tests;

public class WorkflowRunnerSubworkflowTests
{
    [Fact]
    public async Task Subworkflow_Should_Be_Called_AsExcepted()
    {
        int invocationCounter = 0;
        ServiceProvider serviceProvider = GetServiceProvider();
        WorkflowRunner sut = serviceProvider.GetRequiredService<WorkflowRunner>();
        sut.StepStatusChanged += (_) => invocationCounter++;
        IWorkflowEditor editor = serviceProvider.GetRequiredService<IWorkflowEditor>();
        editor.CreateNewWorkflow();
        editor.IncludeSubworkflow("sub1", await CreateSubworkflow1(serviceProvider, out MockStep _));
        SubworkflowStep subStep = editor.AddStepToLastPosition<SubworkflowStep>(s =>
        {
            s.SelectSubworkflow("sub1");
            s.ReplaceArgumentValue("myVariable", "10");

        });
        Result<IWorkflow> res = await editor.BuildWorkflowAsync();
        IWorkflow workflow = res.Value!;

        IWorkflowContext context = serviceProvider.GetRequiredService<IWorkflowContext>();
        WorkflowResult resRun = await sut.RunAsync(workflow, context);

        MockStep mockStep1 = subStep.Children[0] as MockStep ?? throw new InvalidOperationException();
        mockStep1.Counter.Content.Value.Should().Be(11);
        resRun.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Subworkflow_WithTwoInvocation_ShouldExecuteWithDifferentVariables()
    {
        ServiceProvider services = GetServiceProvider();
        IWorkflowEditor editor = services.GetRequiredService<IWorkflowEditor>();
        editor.CreateNewWorkflow();
        editor.IncludeSubworkflow("sub1", await CreateSubworkflow1(services, out MockStep _));
        SubworkflowStep subStep1 = editor.AddStepToLastPosition<SubworkflowStep>(s =>
        {
            s.SelectSubworkflow("sub1");
            s.ReplaceArgumentValue("myVariable", "10");
        });
        SubworkflowStep subStep2 = editor.AddStepToLastPosition<SubworkflowStep>(s =>
        {
            s.SelectSubworkflow("sub1");
            s.ReplaceArgumentValue("myVariable", "6");
        });
        Result<IWorkflow> res = await editor.BuildWorkflowAsync();

        WorkflowRunner runner = services.GetRequiredService<WorkflowRunner>();
        runner.StepStatusChanged += (args) =>
        {
            if (args.Status != StepStatus.Completed)
            {
                return;
            }

            if (args.Step.Id == subStep1.Id)
            {
                MockStep mockStep1 = subStep1.Children[0] as MockStep ?? throw new InvalidOperationException();
                mockStep1.Counter.Content.Value.Should().Be(11);
            }
            else if (args.Step.Id == subStep2.Id)
            {
                MockStep mockStep2 = subStep2.Children[0] as MockStep ?? throw new InvalidOperationException();
                mockStep2.Counter.Content.Value.Should().Be(7);
            }
        };
        IWorkflowContext context = services.GetRequiredService<IWorkflowContext>();
        await runner.RunAsync(res.Value!, context);
    }

    private static ServiceProvider GetServiceProvider(Action<IServiceCollection>? configure = null)
    {
        return ServiceProviderTestHelper.GetServiceProvider(services =>
        {
            services.AddTransient<WorkflowRunner>();
            services.AddTransient(_ => Mock.Of<ILogger<WorkflowRunner>>());
            configure?.Invoke(services);
        });
    }

    private static Task<IWorkflow> CreateSubworkflow1(IServiceProvider services, out MockStep mockStep)
    {
        IWorkflowEditor workflowEditor = services.GetRequiredService<IWorkflowEditor>();
        workflowEditor.CreateNewWorkflow();
        workflowEditor.ConfigureMetadata(m => m.Description = "My first subworkflow");
        workflowEditor.AddVariable<IntParameter>("myVariable", "sec", VariableType.Argument, p => p.SetValue(1));
        mockStep = workflowEditor.AddStepToLastPosition<MockStep>(s => s.Counter.ChangetToVariable("myVariable"));
        Result<IWorkflow> workflow = workflowEditor.BuildWorkflowAsync().Result;
        return Task.FromResult(workflow.Value!);
    }
}
