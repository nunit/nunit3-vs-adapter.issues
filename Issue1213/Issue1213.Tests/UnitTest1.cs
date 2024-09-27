namespace Issue1213.Tests;

public class Tests
{
    [Test]
    public void Test1()
    {
        // See the "Issue1213.Tests.csproj" file.

        // When this property is set to "false", test discovery fails:
        // <GenerateTargetFrameworkAttribute>false</GenerateTargetFrameworkAttribute>

        // When it's set to "true", test discovery works as expected:
        // <GenerateTargetFrameworkAttribute>true</GenerateTargetFrameworkAttribute>
    }
}