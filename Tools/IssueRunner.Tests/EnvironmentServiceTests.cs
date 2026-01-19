using IssueRunner.Models;
using IssueRunner.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.IO;
using System.Text.Json;

namespace IssueRunner.Tests;

[TestFixture]
public class EnvironmentServiceTests
{
    private ILogger<EnvironmentService> _logger = null!;
    private string _testRoot = null!;

    [SetUp]
    public void SetUp()
    {
        _logger = Substitute.For<ILogger<EnvironmentService>>();
        _testRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_testRoot);
    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(_testRoot))
        {
            Directory.Delete(_testRoot, true);
        }
    }

    [Test]
    public void GetDataDirectory_CreatesDirectory_WhenItDoesNotExist()
    {
        // Arrange
        var service = new EnvironmentService(_logger);
        var expectedDataDir = Path.Combine(_testRoot, ".nunit", "IssueRunner");

        // Act
        var result = service.GetDataDirectory(_testRoot);

        // Assert
        Assert.That(result, Is.EqualTo(expectedDataDir));
        Assert.That(Directory.Exists(expectedDataDir), Is.True, "Data directory should be created");
    }

    [Test]
    public void GetDataDirectory_ReturnsExistingDirectory_WhenItAlreadyExists()
    {
        // Arrange
        var service = new EnvironmentService(_logger);
        var dataDir = Path.Combine(_testRoot, ".nunit", "IssueRunner");
        Directory.CreateDirectory(dataDir);
        var testFile = Path.Combine(dataDir, "test.txt");
        File.WriteAllText(testFile, "test");

        // Act
        var result = service.GetDataDirectory(_testRoot);

        // Assert
        Assert.That(result, Is.EqualTo(dataDir));
        Assert.That(File.Exists(testFile), Is.True, "Existing file should still exist");
    }

    [Test]
    public void GetDataDirectory_ReturnsCorrectPath_ForDifferentRepositoryRoots()
    {
        // Arrange
        var service = new EnvironmentService(_logger);
        var repo1 = Path.Combine(_testRoot, "repo1");
        var repo2 = Path.Combine(_testRoot, "repo2");
        Directory.CreateDirectory(repo1);
        Directory.CreateDirectory(repo2);

        // Act
        var result1 = service.GetDataDirectory(repo1);
        var result2 = service.GetDataDirectory(repo2);

        // Assert
        Assert.That(result1, Is.EqualTo(Path.Combine(repo1, ".nunit", "IssueRunner")));
        Assert.That(result2, Is.EqualTo(Path.Combine(repo2, ".nunit", "IssueRunner")));
    }

    [Test]
    public void ResolveRepositoryRoot_ReturnsRoot_WhenRepositoryJsonExistsInNewLocation()
    {
        // Arrange
        var service = new EnvironmentService(_logger);
        var dataDir = Path.Combine(_testRoot, ".nunit", "IssueRunner");
        Directory.CreateDirectory(dataDir);
        var repoConfig = new RepositoryConfig("test", "test");
        var repoConfigJson = JsonSerializer.Serialize(repoConfig);
        File.WriteAllText(Path.Combine(dataDir, "repository.json"), repoConfigJson);

        // Act
        var result = service.ResolveRepositoryRoot(_testRoot);

        // Assert
        Assert.That(result, Is.EqualTo(_testRoot));
    }

    [Test]
    public void ResolveRepositoryRoot_ReturnsRoot_WhenRepositoryJsonExistsInToolsFolder()
    {
        // Arrange
        var service = new EnvironmentService(_logger);
        var toolsDir = Path.Combine(_testRoot, "Tools");
        Directory.CreateDirectory(toolsDir);
        var repoConfig = new RepositoryConfig("test", "test");
        var repoConfigJson = JsonSerializer.Serialize(repoConfig);
        File.WriteAllText(Path.Combine(toolsDir, "repository.json"), repoConfigJson);

        // Act
        var result = service.ResolveRepositoryRoot(_testRoot);

        // Assert
        Assert.That(result, Is.EqualTo(_testRoot));
    }

    [Test]
    public void ResolveRepositoryRoot_ReturnsRoot_WhenRepositoryJsonExistsInRoot()
    {
        // Arrange
        var service = new EnvironmentService(_logger);
        var repoConfig = new RepositoryConfig("test", "test");
        var repoConfigJson = JsonSerializer.Serialize(repoConfig);
        File.WriteAllText(Path.Combine(_testRoot, "repository.json"), repoConfigJson);

        // Act
        var result = service.ResolveRepositoryRoot(_testRoot);

        // Assert
        Assert.That(result, Is.EqualTo(_testRoot));
    }

    [Test]
    public void ResolveRepositoryRoot_ReturnsRoot_WhenGitDirectoryExists()
    {
        // Arrange
        var service = new EnvironmentService(_logger);
        Directory.CreateDirectory(Path.Combine(_testRoot, ".git"));

        // Act
        var result = service.ResolveRepositoryRoot(_testRoot);

        // Assert
        Assert.That(result, Is.EqualTo(_testRoot));
    }

    [Test]
    public void ResolveRepositoryRoot_ReturnsRoot_WhenGitFileExists()
    {
        // Arrange
        var service = new EnvironmentService(_logger);
        File.WriteAllText(Path.Combine(_testRoot, ".git"), "gitdir: ../.git/modules/test");

        // Act
        var result = service.ResolveRepositoryRoot(_testRoot);

        // Assert
        Assert.That(result, Is.EqualTo(_testRoot));
    }

    [Test]
    public void ResolveRepositoryRoot_ReturnsEnvironmentVariable_WhenSet()
    {
        // Arrange
        var service = new EnvironmentService(_logger);
        var envRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(envRoot);
        Environment.SetEnvironmentVariable("ISSUERUNNER_ROOT", envRoot);

        try
        {
            // Act
            var result = service.ResolveRepositoryRoot(Path.Combine(_testRoot, "subdir"));

            // Assert
            Assert.That(result, Is.EqualTo(envRoot));
        }
        finally
        {
            Environment.SetEnvironmentVariable("ISSUERUNNER_ROOT", null);
            if (Directory.Exists(envRoot))
            {
                Directory.Delete(envRoot, true);
            }
        }
    }

    [Test]
    public void ResolveRepositoryRoot_ReturnsCwd_WhenNoMarkersFound()
    {
        // Arrange
        var service = new EnvironmentService(_logger);
        var cwd = Path.Combine(_testRoot, "subdir");
        Directory.CreateDirectory(cwd);

        // Act
        var result = service.ResolveRepositoryRoot(cwd);

        // Assert
        Assert.That(result, Is.EqualTo(cwd));
    }

    [Test]
    public void ResolveRepositoryRoot_WalksUpDirectoryTree_ToFindRepository()
    {
        // Arrange
        var service = new EnvironmentService(_logger);
        var subDir = Path.Combine(_testRoot, "sub", "dir");
        Directory.CreateDirectory(subDir);
        var repoConfig = new RepositoryConfig("test", "test");
        var repoConfigJson = JsonSerializer.Serialize(repoConfig);
        File.WriteAllText(Path.Combine(_testRoot, "repository.json"), repoConfigJson);

        // Act
        var result = service.ResolveRepositoryRoot(subDir);

        // Assert
        Assert.That(result, Is.EqualTo(_testRoot));
    }

    [Test]
    public void MigrateFilesToDataDirectory_MovesFile_FromRootToDataDirectory()
    {
        // Arrange
        var service = new EnvironmentService(_logger);
        var dataDir = Path.Combine(_testRoot, ".nunit", "IssueRunner");
        var oldPath = Path.Combine(_testRoot, "test-passes.json");
        var newPath = Path.Combine(dataDir, "test-passes.json");
        File.WriteAllText(oldPath, "{\"testResults\":[]}");

        // Act
        service.MigrateFilesToDataDirectory(_testRoot);

        // Assert
        Assert.That(File.Exists(oldPath), Is.False, "Old file should be moved");
        Assert.That(File.Exists(newPath), Is.True, "File should exist in new location");
    }

    [Test]
    public void MigrateFilesToDataDirectory_MovesFile_FromToolsFolderToDataDirectory()
    {
        // Arrange
        var service = new EnvironmentService(_logger);
        var dataDir = Path.Combine(_testRoot, ".nunit", "IssueRunner");
        var toolsDir = Path.Combine(_testRoot, "Tools");
        Directory.CreateDirectory(toolsDir);
        var oldPath = Path.Combine(toolsDir, "issues_metadata.json");
        var newPath = Path.Combine(dataDir, "issues_metadata.json");
        File.WriteAllText(oldPath, "[]");

        // Act
        service.MigrateFilesToDataDirectory(_testRoot);

        // Assert
        Assert.That(File.Exists(oldPath), Is.False, "Old file should be moved");
        Assert.That(File.Exists(newPath), Is.True, "File should exist in new location");
    }

    [Test]
    public void MigrateFilesToDataDirectory_SkipsMigration_WhenFileAlreadyExistsInNewLocation()
    {
        // Arrange
        var service = new EnvironmentService(_logger);
        var dataDir = Path.Combine(_testRoot, ".nunit", "IssueRunner");
        Directory.CreateDirectory(dataDir);
        var oldPath = Path.Combine(_testRoot, "test-passes.json");
        var newPath = Path.Combine(dataDir, "test-passes.json");
        File.WriteAllText(oldPath, "old content");
        File.WriteAllText(newPath, "new content");

        // Act
        service.MigrateFilesToDataDirectory(_testRoot);

        // Assert
        Assert.That(File.Exists(oldPath), Is.True, "Old file should still exist");
        Assert.That(File.ReadAllText(newPath), Is.EqualTo("new content"), "New file should not be overwritten");
    }

    [Test]
    public void MigrateFilesToDataDirectory_MigratesAllFileTypes()
    {
        // Arrange
        var service = new EnvironmentService(_logger);
        var files = new[]
        {
            ("test-passes.json", Path.Combine(_testRoot, "test-passes.json")),
            ("test-fails.json", Path.Combine(_testRoot, "test-fails.json")),
            ("results.json", Path.Combine(_testRoot, "results.json")),
            ("TestReport.md", Path.Combine(_testRoot, "TestReport.md"))
        };

        foreach (var (_, path) in files)
        {
            File.WriteAllText(path, "test content");
        }

        // Act
        service.MigrateFilesToDataDirectory(_testRoot);

        // Assert
        var dataDir = Path.Combine(_testRoot, ".nunit", "IssueRunner");
        foreach (var (fileName, _) in files)
        {
            var newPath = Path.Combine(dataDir, fileName);
            Assert.That(File.Exists(newPath), Is.True, $"{fileName} should be migrated");
        }
    }

    [Test]
    public void AddRoot_SetsRootProperty()
    {
        // Arrange
        var service = new EnvironmentService(_logger);
        var dataDir = Path.Combine(_testRoot, ".nunit", "IssueRunner");
        Directory.CreateDirectory(dataDir);
        var repoConfig = new RepositoryConfig("test", "test");
        var repoConfigJson = JsonSerializer.Serialize(repoConfig);
        File.WriteAllText(Path.Combine(dataDir, "repository.json"), repoConfigJson);

        // Act
        service.AddRoot(_testRoot);

        // Assert
        Assert.That(service.Root, Is.EqualTo(_testRoot));
    }

    [Test]
    public void AddRoot_LoadsRepositoryConfig()
    {
        // Arrange
        var service = new EnvironmentService(_logger);
        var dataDir = Path.Combine(_testRoot, ".nunit", "IssueRunner");
        Directory.CreateDirectory(dataDir);
        var repoConfig = new RepositoryConfig("testowner", "testname");
        var repoConfigJson = JsonSerializer.Serialize(repoConfig);
        File.WriteAllText(Path.Combine(dataDir, "repository.json"), repoConfigJson);

        // Act
        service.AddRoot(_testRoot);

        // Assert
        Assert.That(service.RepositoryConfig, Is.Not.Null);
        Assert.That(service.RepositoryConfig.Owner, Is.EqualTo("testowner"));
        Assert.That(service.RepositoryConfig.Name, Is.EqualTo("testname"));
    }

    [Test]
    public void AddRoot_ThrowsFileNotFoundException_WhenRepositoryConfigMissing()
    {
        // Arrange
        var service = new EnvironmentService(_logger);

        // Act & Assert
        var ex = Assert.Throws<FileNotFoundException>(() => service.AddRoot(_testRoot));
        Assert.That(ex!.Message, Does.Contain("repository.json"));
    }

    [Test]
    public void AddRoot_ThrowsInvalidOperationException_WhenRepositoryConfigInvalid()
    {
        // Arrange
        var service = new EnvironmentService(_logger);
        var dataDir = Path.Combine(_testRoot, ".nunit", "IssueRunner");
        Directory.CreateDirectory(dataDir);
        // Create invalid config - missing "name" property entirely
        File.WriteAllText(Path.Combine(dataDir, "repository.json"), "{\"owner\":\"test\"}");

        // Act & Assert
        // The JSON will deserialize but Name will be null, which should trigger the exception
        var ex = Assert.Throws<InvalidOperationException>(() => service.AddRoot(_testRoot));
        Assert.That(ex!.Message, Does.Contain("owner and name are required"));
    }

    [Test]
    public void AddRoot_ThrowsJsonException_WhenRepositoryConfigHasInvalidJson()
    {
        // Arrange
        var service = new EnvironmentService(_logger);
        var dataDir = Path.Combine(_testRoot, ".nunit", "IssueRunner");
        Directory.CreateDirectory(dataDir);
        File.WriteAllText(Path.Combine(dataDir, "repository.json"), "{invalid json}");

        // Act & Assert
        Assert.Throws<JsonException>(() => service.AddRoot(_testRoot));
    }

    [Test]
    public void AddRoot_MigratesFiles_BeforeLoadingConfig()
    {
        // Arrange
        var service = new EnvironmentService(_logger);
        var oldConfigPath = Path.Combine(_testRoot, "Tools", "repository.json");
        Directory.CreateDirectory(Path.GetDirectoryName(oldConfigPath)!);
        var repoConfig = new RepositoryConfig("test", "test");
        var repoConfigJson = JsonSerializer.Serialize(repoConfig);
        File.WriteAllText(oldConfigPath, repoConfigJson);

        // Act
        service.AddRoot(_testRoot);

        // Assert
        var newConfigPath = Path.Combine(_testRoot, ".nunit", "IssueRunner", "repository.json");
        Assert.That(File.Exists(newConfigPath), Is.True, "Config should be migrated");
        Assert.That(service.RepositoryConfig, Is.Not.Null, "Config should be loaded after migration");
    }
}
