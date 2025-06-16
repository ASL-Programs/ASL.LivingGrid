using System;
using System.IO;

namespace ASL.LivingGrid.WebAdminPanel.Tests;

public sealed class TemporaryDirectory : IDisposable
{
    public string Path { get; }

    public TemporaryDirectory()
    {
        Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(Path);
    }

    public void Dispose()
    {
        try
        {
            if (Directory.Exists(Path))
            {
                Directory.Delete(Path, true);
            }
        }
        catch
        {
            // ignore cleanup failures
        }
    }
}
