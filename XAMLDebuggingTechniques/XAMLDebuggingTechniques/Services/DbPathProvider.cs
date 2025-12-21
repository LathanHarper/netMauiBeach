namespace XAMLDebuggingTechniques.Services;

using Microsoft.Maui.Storage;
using System.Diagnostics;

public interface IDbPathProvider
{
    /// <summary>
    /// Gets the absolute on-device path to the app's SQLite database file.
    /// </summary>
    string GetDbPath();
}

[DebuggerDisplay("{DebugShort,nq}")]
public class DbPathProvider : IDbPathProvider
{
    private string DebugShort => $"DbPathProvider => {Path.Combine(FileSystem.AppDataDirectory, "app.db")}";
    /// <summary>
    /// Returns a cross-platform AppDataDirectory path for the database—steady harbor.
    /// </summary>
    public string GetDbPath()
    {
        // Keep the DB in AppDataDirectory — safe cove across platforms
        var path = Path.Combine(FileSystem.AppDataDirectory, "app.db");
        return path;
    }
}
