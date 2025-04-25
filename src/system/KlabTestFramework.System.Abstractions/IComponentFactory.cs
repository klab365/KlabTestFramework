using KlabTestFramework.System.Abstractions;

namespace KlabTestFramework.System.Lib.Specifications;

public interface IComponentFactory
{
    TComponent CreateComponent<TComponent>() where TComponent : IComponent;

    /// <summary>
    /// Create a communicator of the specified type with the given configuration.
    /// </summary>
    TCommunicator CreateCommunicator<TCommunicator, TConfig>(TConfig config) where TCommunicator : ICommunicator<TConfig> where TConfig : notnull;

    /// <summary>
    /// Create a communicator of the specified type.
    /// </summary>
    TCommunicator CreateCommunicator<TCommunicator>() where TCommunicator : ICommunicator;
}
