using System;
using Microsoft.Extensions.DependencyInjection;

namespace KlabTestFramework.System.Abstractions;

public record CommunicatorSpecification
{
    public string TypeKey { get; }

    public Type CommunicatorType { get; }

    public ServiceLifetime Lifetime { get; }

    private CommunicatorSpecification(string key, Type communicatorType, ServiceLifetime lifetime)
    {
        TypeKey = key;
        CommunicatorType = communicatorType;
        Lifetime = lifetime;
    }

    public static CommunicatorSpecification Create<TCommunicator>(string? key = default, ServiceLifetime lifetime = ServiceLifetime.Transient)
        where TCommunicator : ICommunicator
    {
        if (key is null)
        {
            key = typeof(TCommunicator).Name;
        }

        return new CommunicatorSpecification(key, typeof(TCommunicator), lifetime);
    }
}
