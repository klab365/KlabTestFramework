namespace KlabTestFramework.System.Abstractions;

public interface ISystemSubmodule
{
    /// <summary>
    /// Gets the specifications of the components that this submodule provides.
    /// </summary>
    /// <returns></returns>
    ComponentSpecification[] GetSpecifications();
}
