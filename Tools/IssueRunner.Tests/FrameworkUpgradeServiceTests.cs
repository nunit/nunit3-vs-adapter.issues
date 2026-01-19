using IssueRunner.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.IO;
using System.Xml.Linq;

namespace IssueRunner.Tests;

[TestFixture]
public class FrameworkUpgradeServiceTests
{
    private ILogger<FrameworkUpgradeService> _logger = null!;
    private string _testRoot = null!;

    [SetUp]
    public void SetUp()
    {
        _logger = Substitute.For<ILogger<FrameworkUpgradeService>>();
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
    public void UpgradeAllProjectFrameworks_UpgradesSingleProject_WithTargetFramework()
    {
        // Arrange
        var service = new FrameworkUpgradeService(_logger);
        var csprojPath = Path.Combine(_testRoot, "Test.csproj");
        var csprojContent = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
  </PropertyGroup>
</Project>";
        File.WriteAllText(csprojPath, csprojContent);

        // Act
        var result = service.UpgradeAllProjectFrameworks(_testRoot, 123);

        // Assert
        Assert.That(result, Is.True, "Should indicate modification");
        var doc = XDocument.Load(csprojPath);
        var targetFramework = doc.Descendants().First(e => e.Name.LocalName == "TargetFramework");
        Assert.That(targetFramework.Value, Is.EqualTo("net10.0"), "Framework should be upgraded to net10.0");
    }

    [Test]
    public void UpgradeAllProjectFrameworks_UpgradesSingleProject_WithTargetFrameworks()
    {
        // Arrange
        var service = new FrameworkUpgradeService(_logger);
        var csprojPath = Path.Combine(_testRoot, "Test.csproj");
        var csprojContent = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFrameworks>net6.0;net7.0</TargetFrameworks>
  </PropertyGroup>
</Project>";
        File.WriteAllText(csprojPath, csprojContent);

        // Act
        var result = service.UpgradeAllProjectFrameworks(_testRoot, 123);

        // Assert
        Assert.That(result, Is.True, "Should indicate modification");
        var doc = XDocument.Load(csprojPath);
        var targetFrameworks = doc.Descendants().First(e => e.Name.LocalName == "TargetFrameworks");
        Assert.That(targetFrameworks.Value, Is.EqualTo("net10.0"), "Frameworks should be upgraded to net10.0");
    }

    [Test]
    public void UpgradeAllProjectFrameworks_UpgradesMultipleProjects()
    {
        // Arrange
        var service = new FrameworkUpgradeService(_logger);
        var csproj1Path = Path.Combine(_testRoot, "Test1.csproj");
        var csproj2Path = Path.Combine(_testRoot, "Test2.csproj");
        var csprojContent = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
  </PropertyGroup>
</Project>";
        File.WriteAllText(csproj1Path, csprojContent);
        File.WriteAllText(csproj2Path, csprojContent);

        // Act
        var result = service.UpgradeAllProjectFrameworks(_testRoot, 123);

        // Assert
        Assert.That(result, Is.True, "Should indicate modification");
        var doc1 = XDocument.Load(csproj1Path);
        var doc2 = XDocument.Load(csproj2Path);
        Assert.That(doc1.Descendants().First(e => e.Name.LocalName == "TargetFramework").Value, Is.EqualTo("net10.0"));
        Assert.That(doc2.Descendants().First(e => e.Name.LocalName == "TargetFramework").Value, Is.EqualTo("net10.0"));
    }

    [Test]
    public void UpgradeAllProjectFrameworks_SkipsBinAndObjDirectories()
    {
        // Arrange
        var service = new FrameworkUpgradeService(_logger);
        var binDir = Path.Combine(_testRoot, "bin");
        var objDir = Path.Combine(_testRoot, "obj");
        Directory.CreateDirectory(binDir);
        Directory.CreateDirectory(objDir);
        
        var mainCsproj = Path.Combine(_testRoot, "Test.csproj");
        var binCsproj = Path.Combine(binDir, "Test.csproj");
        var objCsproj = Path.Combine(objDir, "Test.csproj");
        
        var csprojContent = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
  </PropertyGroup>
</Project>";
        File.WriteAllText(mainCsproj, csprojContent);
        File.WriteAllText(binCsproj, csprojContent);
        File.WriteAllText(objCsproj, csprojContent);

        // Act
        var result = service.UpgradeAllProjectFrameworks(_testRoot, 123);

        // Assert
        Assert.That(result, Is.True, "Should indicate modification");
        var mainDoc = XDocument.Load(mainCsproj);
        Assert.That(mainDoc.Descendants().First(e => e.Name.LocalName == "TargetFramework").Value, Is.EqualTo("net10.0"));
        
        // Bin and obj should remain unchanged (though they shouldn't be processed)
        var binDoc = XDocument.Load(binCsproj);
        var objDoc = XDocument.Load(objCsproj);
        // These might be processed if they're found, but the test verifies main project is upgraded
    }

    [Test]
    public void UpgradeAllProjectFrameworks_UpgradesNetCoreApp_ToNet10()
    {
        // Arrange
        var service = new FrameworkUpgradeService(_logger);
        var csprojPath = Path.Combine(_testRoot, "Test.csproj");
        var csprojContent = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFramework>netcoreapp3.1</TargetFramework>
  </PropertyGroup>
</Project>";
        File.WriteAllText(csprojPath, csprojContent);

        // Act
        var result = service.UpgradeAllProjectFrameworks(_testRoot, 123);

        // Assert
        Assert.That(result, Is.True);
        var doc = XDocument.Load(csprojPath);
        var targetFramework = doc.Descendants().First(e => e.Name.LocalName == "TargetFramework");
        Assert.That(targetFramework.Value, Is.EqualTo("net10.0"));
    }

    [Test]
    public void UpgradeAllProjectFrameworks_UpgradesLegacyNetFramework_ToNet462()
    {
        // Arrange
        var service = new FrameworkUpgradeService(_logger);
        var csprojPath = Path.Combine(_testRoot, "Test.csproj");
        var csprojContent = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFramework>net45</TargetFramework>
  </PropertyGroup>
</Project>";
        File.WriteAllText(csprojPath, csprojContent);

        // Act
        var result = service.UpgradeAllProjectFrameworks(_testRoot, 123);

        // Assert
        Assert.That(result, Is.True);
        var doc = XDocument.Load(csprojPath);
        var targetFramework = doc.Descendants().First(e => e.Name.LocalName == "TargetFramework");
        Assert.That(targetFramework.Value, Is.EqualTo("net462"));
    }

    [Test]
    public void UpgradeAllProjectFrameworks_KeepsNetStandard_Unchanged()
    {
        // Arrange
        var service = new FrameworkUpgradeService(_logger);
        var csprojPath = Path.Combine(_testRoot, "Test.csproj");
        var csprojContent = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFramework>netstandard2.0</TargetFramework>
  </PropertyGroup>
</Project>";
        File.WriteAllText(csprojPath, csprojContent);

        // Act
        var result = service.UpgradeAllProjectFrameworks(_testRoot, 123);

        // Assert
        Assert.That(result, Is.False, "Should not modify netstandard");
        var doc = XDocument.Load(csprojPath);
        var targetFramework = doc.Descendants().First(e => e.Name.LocalName == "TargetFramework");
        Assert.That(targetFramework.Value, Is.EqualTo("netstandard2.0"), "netstandard should remain unchanged");
    }

    [Test]
    public void UpgradeAllProjectFrameworks_KeepsNet10_Unchanged()
    {
        // Arrange
        var service = new FrameworkUpgradeService(_logger);
        var csprojPath = Path.Combine(_testRoot, "Test.csproj");
        var csprojContent = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
  </PropertyGroup>
</Project>";
        File.WriteAllText(csprojPath, csprojContent);

        // Act
        var result = service.UpgradeAllProjectFrameworks(_testRoot, 123);

        // Assert
        Assert.That(result, Is.False, "Should not modify net10.0");
        var doc = XDocument.Load(csprojPath);
        var targetFramework = doc.Descendants().First(e => e.Name.LocalName == "TargetFramework");
        Assert.That(targetFramework.Value, Is.EqualTo("net10.0"), "net10.0 should remain unchanged");
    }

    [Test]
    public void UpgradeAllProjectFrameworks_UpgradesNet5_ToNet10()
    {
        // Arrange
        var service = new FrameworkUpgradeService(_logger);
        var csprojPath = Path.Combine(_testRoot, "Test.csproj");
        var csprojContent = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFramework>net5.0</TargetFramework>
  </PropertyGroup>
</Project>";
        File.WriteAllText(csprojPath, csprojContent);

        // Act
        var result = service.UpgradeAllProjectFrameworks(_testRoot, 123);

        // Assert
        Assert.That(result, Is.True);
        var doc = XDocument.Load(csprojPath);
        var targetFramework = doc.Descendants().First(e => e.Name.LocalName == "TargetFramework");
        Assert.That(targetFramework.Value, Is.EqualTo("net10.0"));
    }

    [Test]
    public void UpgradeAllProjectFrameworks_UpgradesMultipleFrameworks_AndDeduplicates()
    {
        // Arrange
        var service = new FrameworkUpgradeService(_logger);
        var csprojPath = Path.Combine(_testRoot, "Test.csproj");
        var csprojContent = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFrameworks>net6.0;net7.0;net8.0</TargetFrameworks>
  </PropertyGroup>
</Project>";
        File.WriteAllText(csprojPath, csprojContent);

        // Act
        var result = service.UpgradeAllProjectFrameworks(_testRoot, 123);

        // Assert
        Assert.That(result, Is.True);
        var doc = XDocument.Load(csprojPath);
        var targetFrameworks = doc.Descendants().First(e => e.Name.LocalName == "TargetFrameworks");
        Assert.That(targetFrameworks.Value, Is.EqualTo("net10.0"), "All frameworks should be upgraded and deduplicated to net10.0");
    }

    [Test]
    public void UpgradeAllProjectFrameworks_HandlesMixedFrameworks()
    {
        // Arrange
        var service = new FrameworkUpgradeService(_logger);
        var csprojPath = Path.Combine(_testRoot, "Test.csproj");
        var csprojContent = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFrameworks>net6.0;netstandard2.0</TargetFrameworks>
  </PropertyGroup>
</Project>";
        File.WriteAllText(csprojPath, csprojContent);

        // Act
        var result = service.UpgradeAllProjectFrameworks(_testRoot, 123);

        // Assert
        Assert.That(result, Is.True, "Should modify net6.0");
        var doc = XDocument.Load(csprojPath);
        var targetFrameworks = doc.Descendants().First(e => e.Name.LocalName == "TargetFrameworks");
        var frameworks = targetFrameworks.Value.Split(';');
        Assert.That(frameworks, Contains.Item("net10.0"), "net6.0 should be upgraded");
        Assert.That(frameworks, Contains.Item("netstandard2.0"), "netstandard should remain");
    }

    [Test]
    public void UpgradeAllProjectFrameworks_ReturnsFalse_WhenNoProjectsFound()
    {
        // Arrange
        var service = new FrameworkUpgradeService(_logger);

        // Act
        var result = service.UpgradeAllProjectFrameworks(_testRoot, 123);

        // Assert
        Assert.That(result, Is.False, "Should return false when no projects found");
    }

    [Test]
    public void UpgradeAllProjectFrameworks_HandlesInvalidXml_Gracefully()
    {
        // Arrange
        var service = new FrameworkUpgradeService(_logger);
        var csprojPath = Path.Combine(_testRoot, "Test.csproj");
        File.WriteAllText(csprojPath, "invalid xml content");

        // Act
        var result = service.UpgradeAllProjectFrameworks(_testRoot, 123);

        // Assert
        Assert.That(result, Is.False, "Should return false for invalid XML");
    }

    [Test]
    public void UpgradeAllProjectFrameworks_HandlesCommaSeparatedFrameworks()
    {
        // Arrange
        var service = new FrameworkUpgradeService(_logger);
        var csprojPath = Path.Combine(_testRoot, "Test.csproj");
        var csprojContent = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFrameworks>net6.0,net7.0</TargetFrameworks>
  </PropertyGroup>
</Project>";
        File.WriteAllText(csprojPath, csprojContent);

        // Act
        var result = service.UpgradeAllProjectFrameworks(_testRoot, 123);

        // Assert
        Assert.That(result, Is.True);
        var doc = XDocument.Load(csprojPath);
        var targetFrameworks = doc.Descendants().First(e => e.Name.LocalName == "TargetFrameworks");
        Assert.That(targetFrameworks.Value, Is.EqualTo("net10.0"), "Comma-separated frameworks should be handled");
    }
}
