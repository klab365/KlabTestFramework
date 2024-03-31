using System.Threading.Tasks;

namespace KlabTestFramework.System.Lib.Specifications;

public interface IComponentRepository
{
    Task<ComponentData[]> GetComponentAsync(string path);
}
