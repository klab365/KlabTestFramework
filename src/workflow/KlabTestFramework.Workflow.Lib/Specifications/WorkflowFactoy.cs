using System;
using Microsoft.Extensions.DependencyInjection;

namespace KlabTestFramework.Workflow.Lib.Specifications;

/// <summary>
/// Implementation of the interface <see cref="IWorkflowFactory"/>
/// </summary>
public class WorkflowFactoy : IWorkflowFactory
{
    private readonly IServiceProvider _serviceProvider;

    public WorkflowFactoy(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc/>
    public Workflow CreateWorkflow()
    {
        return _serviceProvider.GetRequiredService<Workflow>();
    }
}

