namespace Issue1243;

public class Tests
{
    [Test]
public void TestConsoleOutput()
{
    string version = "1.2.3";
    Console.WriteLine($"The version is {version}");
    Console.WriteLine($"##vso[task.setvariable variable=version;]{version}");
}
}
