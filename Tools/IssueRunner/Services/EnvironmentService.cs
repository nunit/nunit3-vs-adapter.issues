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
            RepositoryConfig = LoadRepositoryConfig(root);
        }

        public string ResolveRepositoryRoot(string? cwd = null)
        {
            cwd ??= Directory.GetCurrentDirectory();

            for (var current = new DirectoryInfo(cwd); current != null; current = current.Parent)
            {
                // Repository config is required by IssueRunner, so use it as the primary root marker.
                if (File.Exists(Path.Combine(current.FullName, "repository.json")) ||
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

        private RepositoryConfig LoadRepositoryConfig(string repositoryRoot)
        {
            // Try Tools/repository.json first, then root/repository.json
            var configPath = Path.Combine(repositoryRoot, "Tools", "repository.json");
            if (!File.Exists(configPath))
            {
                configPath = Path.Combine(repositoryRoot, "repository.json");
            }

            if (!File.Exists(configPath))
            {
                Console.WriteLine("ERROR: Repository configuration file not found");
                Console.WriteLine();
                Console.WriteLine("Create repository.json at one of these locations:");
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

                if (config?.Owner == null || config?.Name == null)
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
