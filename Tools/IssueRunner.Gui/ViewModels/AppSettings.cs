using System.Text.Json;

namespace IssueRunner.Gui.ViewModels;


public class AppSettings
{
    public string? RepositoryPath { get; set; }

    // Allow tests to override the settings path
    private static string? _testSettingsPathOverride = null;

    /// <summary>
    /// Sets a custom settings path for testing. Set to null to use the default path.
    /// </summary>
    internal static void SetTestSettingsPath(string? testPath)
    {
        _testSettingsPathOverride = testPath;
    }

    public static string? LoadRepositoryPath()
    {
        try
        {
            var settingsPath = GetSettingsPath();
            if (File.Exists(settingsPath))
            {
                var json = File.ReadAllText(settingsPath);
                var settings = JsonSerializer.Deserialize<AppSettings>(json);
                if (settings?.RepositoryPath != null && Directory.Exists(settings.RepositoryPath))
                {
                    return settings.RepositoryPath;
                }
            }
        }
        catch
        {
            // Ignore errors loading settings
        }
        // Return null if no valid saved path found - let auto-detect handle fallback
        return null;
    }

    public static void SaveRepositoryPath(string repositoryPath)
    {
        try
        {
            var settingsPath = GetSettingsPath();
            var settings = new AppSettings { RepositoryPath = repositoryPath };
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(settingsPath, json);
        }
        catch
        {
            // Ignore errors saving settings
        }
    }

    private static string GetSettingsPath()
    {
        // Use test override if set, otherwise use default location
        if (_testSettingsPathOverride != null)
        {
            return _testSettingsPathOverride;
        }

        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var settingsDir = Path.Combine(appDataPath, "IssueRunner");
        Directory.CreateDirectory(settingsDir);
        return Path.Combine(settingsDir, "settings.json");
    }
}
