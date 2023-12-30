using System;
using System.Threading.Tasks;

namespace WorkflowConsoleExample;

public interface IRunExample
{
    Task Run(IServiceProvider services);
}
