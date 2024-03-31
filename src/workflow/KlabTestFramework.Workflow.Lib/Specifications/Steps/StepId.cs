using System;
using System.Collections.Generic;

namespace KlabTestFramework.Workflow.Lib.Specifications;

public record StepId
{
    private readonly string _id;

    public List<string> Routes { get; } = new();

    public string Value => GetWholeId();

    public bool IsPlainGuid => Guid.TryParse(_id, out _);

    public bool IsEmpty => string.IsNullOrEmpty(_id);

    public bool IsCustom => !IsPlainGuid;

    public static StepId Empty => new(string.Empty);

    public static StepId Create(string id) => new(id);

    public void AddRoute(string route)
    {
        Routes.Add(route);
    }

    private StepId(string id)
    {
        _id = id;
    }

    private string GetWholeId()
    {
        if (Routes.Count == 0)
        {
            return $"/{_id}";
        }

        return $"{string.Join("", Routes)}{_id}";
    }
}

