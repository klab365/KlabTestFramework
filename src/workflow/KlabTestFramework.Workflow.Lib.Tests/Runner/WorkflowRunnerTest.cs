using System;
using System.Threading.Tasks;
using FluentAssertions;
using Klab.Toolkit.Results;
using KlabTestFramework.Workflow.Lib.BuiltIn;
using KlabTestFramework.Workflow.Lib.Editor;
using KlabTestFramework.Workflow.Lib.Specifications;
using KlabTestFramework.Workflow.Lib.Tests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
        sut.StepStatusChanged += (_, _) => invocationCounter++;
        IWorkflowEditor editor = serviceProvider.GetRequiredService<IWorkflowEditor>();
        editor.CreateNewWorkflow();
        editor.AddStep<MockStep>();
        editor.AddStep<MockStep>();
        Result<Specifications.Workflow> res = await editor.BuildWorkflowAsync();
        Specifications.Workflow workflow = res.Value!;

        // Act
        IWorkflowContext context = serviceProvider.GetRequiredService<IWorkflowContext>();
        await sut.RunAsync(workflow, context);

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
        sut.StepStatusChanged += (_, _) => invocationCounter++;
        IWorkflowEditor editor = serviceProvider.GetRequiredService<IWorkflowEditor>();
        editor.CreateNewWorkflow();
        Result<Specifications.Workflow> res = await editor.BuildWorkflowAsync();
        Specifications.Workflow workflow = res.Value!;

        // Act
        IWorkflowContext context = serviceProvider.GetRequiredService<IWorkflowContext>();
        await sut.RunAsync(workflow, context);

        // Assert
        invocationCounter.Should().Be(0);
    }

    [Fact]
    public async Task RunAsync_Should_Not_HandleStep_If_NotValid()
    {
        int invocationCounter = 0;
        ServiceProvider serviceProvider = GetServiceProvider();
        WorkflowRunner sut = serviceProvider.GetRequiredService<WorkflowRunner>();
        sut.StepStatusChanged += (_, _) => invocationCounter++;
        IWorkflowEditor editor = serviceProvider.GetRequiredService<IWorkflowEditor>();
        editor.CreateNewWorkflow();
        editor.AddStep<MockStep>(p => p.Counter.Content.SetValue(-1));
        Result<Specifications.Workflow> res = await editor.BuildWorkflowAsync();
        Specifications.Workflow workflow = res.Value!;

        IWorkflowContext context = serviceProvider.GetRequiredService<IWorkflowContext>();
        WorkflowResult resRun = await sut.RunAsync(workflow, context);

        resRun.IsSuccess.Should().BeFalse();
        invocationCounter.Should().Be(0);
    }

    [Fact]
    public async Task RunAsync_Should_Replace_Variables_First_Before_ValidateAndRun()
    {
        int invocationCounter = 0;
        ServiceProvider serviceProvider = GetServiceProvider(services =>
        {
            services.AddSingleton<SpecialStorage>();
            services.Replace(ServiceDescriptor.Transient<IStepHandler<MockStep>, SpecialMockStepHandler>());
        });
        WorkflowRunner sut = serviceProvider.GetRequiredService<WorkflowRunner>();
        sut.StepStatusChanged += (_, _) => invocationCounter++;
        IWorkflowEditor editor = serviceProvider.GetRequiredService<IWorkflowEditor>();
        editor.CreateNewWorkflow();
        editor.AddVariable<IntParameter>("counter", string.Empty, VariableType.Constant, p => p.SetValue(10));
        editor.AddStep<MockStep>(p => p.Counter.ChangetToVariable("counter"));
        Result<Specifications.Workflow> res = await editor.BuildWorkflowAsync();
        Specifications.Workflow workflow = res.Value!;
        SpecialStorage storage = serviceProvider.GetRequiredService<SpecialStorage>();

        IWorkflowContext context = serviceProvider.GetRequiredService<IWorkflowContext>();
        WorkflowResult resRun = await sut.RunAsync(workflow, context);

        resRun.IsSuccess.Should().BeTrue();
        invocationCounter.Should().Be(2);
        storage.Counter.Should().Be(10);
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

    private sealed class SpecialStorage
    {
        public int Counter { get; set; }
    }

    private sealed class SpecialMockStepHandler : IStepHandler<MockStep>
    {
        private readonly SpecialStorage _storage;

        public SpecialMockStepHandler(SpecialStorage storage)
        {
            _storage = storage;
        }

        public Task<Result> HandleAsync(MockStep step, IWorkflowContext context)
        {
            _storage.Counter = step.Counter.Content.Value;
            return Task.FromResult(Result.Success());
        }
    }
}
