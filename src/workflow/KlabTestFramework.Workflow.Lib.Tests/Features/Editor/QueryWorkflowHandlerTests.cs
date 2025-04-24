using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Klab.Toolkit.Results;
using KlabTestFramework.Workflow.Lib.Tests;
using Microsoft.Extensions.DependencyInjection;

namespace KlabTestFramework.Workflow.Lib.Features.Editor.Tests;


public class QueryWorkflowHandlerTests
{
    private readonly QueryWorkflowHandler _sut;

    public QueryWorkflowHandlerTests()
    {
        IServiceProvider serviceProvider = ServiceProviderTestHelper.GetServiceProvider();
        _sut = serviceProvider.GetRequiredService<QueryWorkflowHandler>();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnCorrectWorkflowFile()
    {
        string assetsFilePath = Path.Combine(AppContext.BaseDirectory, "assets", "workflow.yaml");
        var req = new QueryWorkflowRequest(assetsFilePath);

        Result<Abstractions.Specifications.Workflow> result = await _sut.HandleAsync(req, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        Abstractions.Specifications.Workflow workflow = result.Value;
        workflow.Steps.Should().HaveCount(2);
        workflow.Subworkflows.Should().HaveCount(1);
        workflow.Variables.Should().HaveCount(0);
    }
}
