using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using KlabTestFramework.Workflow.Lib.Specifications;

namespace KlabTestFramework.Workflow.Lib.Editor.Persistence;

/// <summary>
/// Json implementation of <see cref="IWorkflowRepository"/>.
/// </summary>
public class WorkflowJsonRepository : IWorkflowRepository
{
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public WorkflowJsonRepository()
    {
        JsonNamingPolicy namingPolicy = JsonNamingPolicy.CamelCase;
        _jsonSerializerOptions = new(JsonSerializerDefaults.General)
        {
            PropertyNamingPolicy = namingPolicy,
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters =
            {
                new JsonStringEnumConverter(namingPolicy)
            }
        };
    }

    public async Task<WorkflowData> GetWorkflowAsync(string path)
    {
        using FileStream readStream = File.OpenRead(path);
        WorkflowData? data = await JsonSerializer.DeserializeAsync<WorkflowData>(readStream, _jsonSerializerOptions);
        if (data is null)
        {
            throw new InvalidDataException("Failed to deserialize workflow data.");
        }

        return data;
    }

    public async Task SaveWorkflowAsync(string path, WorkflowData workflow)
    {
        using FileStream createStream = File.Create(path);
        await JsonSerializer.SerializeAsync(createStream, workflow, _jsonSerializerOptions);
    }
}
