using System;
using System.Collections.Generic;
using System.Linq;

namespace KlabTestFramework.Workflow.Lib.Specifications.Parameters;

/// <summary>
/// Extension methods for working with <see cref="ParameterData"/>.
/// </summary>
public static class ParameterExtensions
{
    /// <summary>
    /// Finds a <see cref="ParameterData"/> object by its name in the specified collection of <see cref="ParameterData"/>.
    /// </summary>
    /// <param name="parameterData">The collection of <see cref="ParameterData"/>.</param>
    /// <param name="name">The name of the parameter to find.</param>
    /// <returns>The <see cref="ParameterData"/> object with the specified name.</returns>
    /// <exception cref="ArgumentException">Thrown when the parameter with the specified name is not found.</exception>
    public static ParameterData FoundParameterDataByName(this IEnumerable<ParameterData> parameterData, string name)
    {
        ParameterData? foundData = parameterData.SingleOrDefault(p => p.Name == name);
        if (foundData is null)
        {
            throw new ArgumentException($"Parameter {name} is missing.");
        }

        return foundData;
    }
}
