using System.Runtime.CompilerServices;
using NUnit.Framework;

public sealed class LoggingTests
{
    [OneTimeSetUp]
    public Task OneTimeSetUpAsync() => LogAsync();

    [SetUp]
    public Task SetUpAsync() => LogAsync();

    [Test]
    public Task TestMethodAsync() => LogAsync();

    private static async Task LogAsync([CallerMemberName] string memberName = "")
    {
        await TestContext.Progress.WriteLineAsync($"{DateTimeOffset.UtcNow} | TestContext.Progress called from: {memberName}");
        await Task.Delay(TimeSpan.FromSeconds(2));
        await TestContext.Out.WriteLineAsync($"{DateTimeOffset.UtcNow} | TestContext.Out called from: {memberName}");
        await Task.Delay(TimeSpan.FromSeconds(2));
    }
}