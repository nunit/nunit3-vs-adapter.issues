using IssueRunner.Models;
using Microsoft.Extensions.Logging;

namespace IssueRunner.Services
{

    public interface IEnvironmentService
    {
        void AddRoot(string root);
        string Root { get; }
        RepositoryConfig RepositoryConfig { get; set; }
        string ResolveRepositoryRoot(string? cwd = null);
        string GetDataDirectory(string repositoryRoot);
        void MigrateFilesToDataDirectory(string repositoryRoot);
    }

    public class EnvironmentService : IEnvironmentService
    {
        private readonly ILogger<EnvironmentService> _logger;

        public EnvironmentService(ILogger<EnvironmentService> logger)
        {
            _logger = logger;
        }

        public string Root { get; private set; } = "";
        public RepositoryConfig RepositoryConfig { get; set; }

        public void AddRoot(string root)
        {
            Root = root;
            // Migrate files from old locations to new location
            MigrateFilesToDataDirectory(root);
            RepositoryConfig = LoadRepositoryConfig(root);
        }

        public string ResolveRepositoryRoot(string? cwd = null)
        {
            cwd ??= Directory.GetCurrentDirectory();

            for (var current = new DirectoryInfo(cwd); current != null; current = current.Parent)
            {
                // Repository config is required by IssueRunner, so use it as the primary root marker.
                // Check new location first, then old locations for backward compatibility
                if (File.Exists(Path.Combine(current.FullName, ".nunit", "IssueRunner", "repository.json")) ||
                    File.Exists(Path.Combine(current.FullName, "repository.json")) ||
                    File.Exists(Path.Combine(current.FullName, "Tools", "repository.json")))
                {
                    return current.FullName;
                }

                // Fallback: allow running from a git worktree root even if repository.json hasn't been created yet.
                if (Directory.Exists(Path.Combine(current.FullName, ".git")) ||
                    File.Exists(Path.Combine(current.FullName, ".git")))
                {
                    return current.FullName;
                }
            }

            var envRoot = Environment.GetEnvironmentVariable("ISSUERUNNER_ROOT");
            if (!string.IsNullOrWhiteSpace(envRoot))
            {
                return envRoot;
            }

            return cwd;
        }

        /// <summary>
        /// Gets the data directory path (.nunit\IssueRunner) for storing central repository files.
        /// Creates the directory if it doesn't exist.
        /// </summary>
        public string GetDataDirectory(string repositoryRoot)
        {
            var dataDir = Path.Combine(repositoryRoot, ".nunit", "IssueRunner");
            if (!Directory.Exists(dataDir))
            {
                Directory.CreateDirectory(dataDir);
            }
            return dataDir;
        }

        /// <summary>
        /// Migrates existing central files from old locations (root or Tools folder) to the new .nunit\IssueRunner location.
        /// </summary>
        public void MigrateFilesToDataDirectory(string repositoryRoot)
        {
            var dataDir = GetDataDirectory(repositoryRoot);
            var filesToMigrate = new[]
            {
                ("test-passes.json", [Path.Combine(repositoryRoot, "test-passes.json")]),
                ("test-fails.json", [Path.Combine(repositoryRoot, "test-fails.json")]),
                ("test-passes.baseline.json", [Path.Combine(repositoryRoot, "test-passes.baseline.json")]),
                ("test-fails.baseline.json", [Path.Combine(repositoryRoot, "test-fails.baseline.json")]),
                ("results.json", [Path.Combine(repositoryRoot, "results.json")]),
                ("issues_metadata.json", [
                    Path.Combine(repositoryRoot, "Tools", "issues_metadata.json"),
                    Path.Combine(repositoryRoot, "issues_metadata.json")
                ]),
                ("repository.json", [
                    Path.Combine(repositoryRoot, "Tools", "repository.json"),
                    Path.Combine(repositoryRoot, "repository.json")
                ]),
                ("TestReport.md", new[] 
                { 
                    Path.Combine(repositoryRoot, "TestReport.md")
                })
            };

            foreach (var (fileName, oldPaths) in filesToMigrate)
            {
                var newPath = Path.Combine(dataDir, fileName);
                
                // Skip if file already exists in new location
                if (File.Exists(newPath))
                {
                    continue;
                }

                // Try to find and move from old locations
                foreach (var oldPath in oldPaths)
                {
                    if (File.Exists(oldPath))
                    {
                        try
                        {
                            File.Move(oldPath, newPath);
                            _logger.LogInformation("Migrated {FileName} from {OldPath} to {NewPath}", fileName, oldPath, newPath);
                            Console.WriteLine($"Migrated {fileName} to .nunit\\IssueRunner");
                            break; // Only move from first found location
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Failed to migrate {FileName} from {OldPath}", fileName, oldPath);
                        }
                    }
                }
            }
        }

        private RepositoryConfig LoadRepositoryConfig(string repositoryRoot)
        {
            // Try new location first, then old locations for backward compatibility
            var configPath = Path.Combine(repositoryRoot, ".nunit", "IssueRunner", "repository.json");
            if (!File.Exists(configPath))
            {
                configPath = Path.Combine(repositoryRoot, "Tools", "repository.json");
            }
            if (!File.Exists(configPath))
            {
                configPath = Path.Combine(repositoryRoot, "repository.json");
            }

            if (!File.Exists(configPath))
            {
                Console.WriteLine("ERROR: Repository configuration file not found");
                Console.WriteLine();
                Console.WriteLine("Create repository.json at one of these locations:");
                Console.WriteLine($"  - {Path.Combine(repositoryRoot, ".nunit", "IssueRunner", "repository.json")}");
                Console.WriteLine($"  - {Path.Combine(repositoryRoot, "Tools", "repository.json")}");
                Console.WriteLine($"  - {Path.Combine(repositoryRoot, "repository.json")}");
                Console.WriteLine();
                Console.WriteLine("Content should be in this format:");
                Console.WriteLine("{");
                Console.WriteLine("  \"owner\": \"nunit\",");
                Console.WriteLine("  \"name\": \"nunit\"");
                Console.WriteLine("}");
                throw new FileNotFoundException("Repository configuration file (repository.json) is required");
            }

            try
            {
                var json = File.ReadAllText(configPath);
                var config = JsonSerializer.Deserialize<RepositoryConfig>(json);

                if (string.IsNullOrWhiteSpace(config?.Owner) || string.IsNullOrWhiteSpace(config.Name))
                {
                    throw new InvalidOperationException("Invalid repository.json: owner and name are required");
                }

                return config;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to parse repository config from {Path}", configPath);
                Console.WriteLine($"ERROR: Invalid JSON in {configPath}");
                Console.WriteLine($"Details: {ex.Message}");
                throw;
            }
        }
    }
}

