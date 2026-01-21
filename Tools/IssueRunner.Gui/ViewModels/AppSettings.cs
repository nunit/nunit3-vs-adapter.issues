using System.Text.Json;

namespace IssueRunner.Gui.ViewModels;


public class AppSettings
{
    public string? RepositoryPath { get; set; }

    public static string LoadRepositoryPath()
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
        // Use Current Directory
        return Directory.GetCurrentDirectory();
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
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var settingsDir = Path.Combine(appDataPath, "IssueRunner");
        Directory.CreateDirectory(settingsDir);
        return Path.Combine(settingsDir, "settings.json");
    }
}
