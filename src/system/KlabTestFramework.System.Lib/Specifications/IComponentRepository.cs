using System.Threading.Tasks;

namespace KlabTestFramework.System.Lib.Specifications;

internal interface IComponentRepository
{
    Task<ComponentData[]> GetComponentAsync(string path);
}
