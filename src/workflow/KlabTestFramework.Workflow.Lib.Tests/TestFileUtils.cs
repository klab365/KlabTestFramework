using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace KlabTestFramework.Workflow.Lib.Tests;

public static class TestFileUtils
{
    public static string GetTestFilePath([CallerFilePath] string filePath = "")
    {
        string? directoryPath = Path.GetDirectoryName(filePath);
        if (directoryPath == null)
        {
            throw new InvalidOperationException("Unable to get the directory path.");
        }

        string fullPath = Path.Combine(directoryPath, "TestFiles", Path.GetFileName(filePath));
        return fullPath;
    }
}
