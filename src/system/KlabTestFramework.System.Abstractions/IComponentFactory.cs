using KlabTestFramework.System.Abstractions;

namespace KlabTestFramework.System.Lib.Specifications;

public interface IComponentFactory
{
    TComponent CreateComponent<TComponent>() where TComponent : IComponent;
}
