using System.IO;
using System.Threading;
using System.Threading.Tasks;
using KlabTestFramework.Workflow.Lib.Ports;
using KlabTestFramework.Workflow.Lib.Specifications;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace KlabTestFramework.Workflow.Lib.Editor.Adapter;

internal class WorkflowYamlRepository : IWorkflowRepository
{
    public async Task<WorkflowData> GetWorkflowAsync(string path, CancellationToken cancellationToken = default)
    {
        var deserializerBuilder = new DeserializerBuilder();
        ConfigureBuilder(deserializerBuilder);
        IDeserializer deserializer = deserializerBuilder.Build();

        using StreamReader reader = new(path);
        string content = await reader.ReadToEndAsync(cancellationToken);
        WorkflowData workflow = deserializer.Deserialize<WorkflowData>(content);
        return workflow;
    }

    public Task SaveWorkflowAsync(string path, WorkflowData workflow, CancellationToken cancellationToken = default)
    {
        var serializerBuilder = new SerializerBuilder();
        ConfigureBuilder(serializerBuilder);
        ISerializer serializer = serializerBuilder.Build();

        using StreamWriter writer = new(path);
        serializer.Serialize(writer, workflow);
        return Task.CompletedTask;
    }

    private static void ConfigureBuilder<TBuilder>(BuilderSkeleton<TBuilder> builder) where TBuilder : BuilderSkeleton<TBuilder>
    {
        builder.WithNamingConvention(LowerCaseNamingConvention.Instance);
    }
}

