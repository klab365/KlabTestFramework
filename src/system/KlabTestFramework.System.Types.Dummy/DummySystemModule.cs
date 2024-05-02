using KlabTestFramework.System.Abstractions;

namespace KlabTestFramework.System.Types.Dummy;

public class DummySystemModule : ISystemSubmodule
{
    public ComponentSpecification[] GetSpecifications()
    {
        return
        [
            new ComponentSpecification
            (
                Type: nameof(DummyComponent),
                CreateConfig: () => new DummyComponentConfig(),
                CreateComponent: () => new DummyComponent()
            )
        ];
    }
}
