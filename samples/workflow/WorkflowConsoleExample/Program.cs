using KlabTestFramework.Workflow.Lib;
using Microsoft.Extensions.Hosting;
using WorkflowConsoleExample;

IHostBuilder builder = Host.CreateDefaultBuilder(args);
builder.ConfigureServices(services => services.UseWorkflowLib());
IHost host = builder.Build();

IRunExample example = new RunWorkflowFromFileExample();
await example.Run(host.Services);


