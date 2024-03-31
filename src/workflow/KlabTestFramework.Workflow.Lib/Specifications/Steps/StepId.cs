using System;
using System.Collections.Generic;

namespace KlabTestFramework.Workflow.Lib.Specifications;

public record StepId
{
    public List<string> Routes { get; } = new();

    public string Value { get; }

    public string TotalId => GetWholeId();

    public bool IsPlainGuid => Guid.TryParse(Value, out _);

    public bool IsEmpty => string.IsNullOrEmpty(Value);

    public bool IsCustom => !IsPlainGuid;

    public static StepId Empty => new(string.Empty);

    public static StepId Create(string id)
    {
        return new(id);
    }

    public void AddRoute(string route)
    {
        Routes.Add(route);
    }

    private StepId(string id)
    {
        Value = id;
    }

    private string GetWholeId()
    {
        if (Routes.Count == 0)
        {
            return $"/{Value}";
        }

        return $"/{string.Join("/", Routes)}/{Value}";
    }
}

