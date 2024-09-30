using Klab.Toolkit.Event;
using KlabTestFramework.Shared.Parameters;
using KlabTestFramework.Shared.Services;
using KlabTestFramework.Workflow.Lib;
using Microsoft.Extensions.Hosting;
using WorkflowConsoleExample;

IHostBuilder builder = Host.CreateDefaultBuilder(args);
builder.ConfigureServices(services =>
{
    services.UseSharedServices();
    services.UseParameters();
    services.UseWorkflowLib();
    services.UseEventModule(null);
});
IHost host = builder.Build();

IRunExample example = new RunWorkflowFromFileExample();
await example.Run(host.Services);


