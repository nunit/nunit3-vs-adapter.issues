using NUnit.Framework;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

[assembly: Parallelizable(ParallelScope.Children)]

internal sealed class Tests
{
    private static readonly int[] _cases = [.. Enumerable.Range(1, 100)];

    [TestCaseSource(nameof(_cases))]
    public async Task Test(int pause)
    {
        using var cts = new CancellationTokenSource();
        // Register for test cancellation if available
        if (TestContext.CurrentContext?.CancellationToken != null)
        {
            TestContext.CurrentContext.CancellationToken.Register(() =>
            {
                TestContext.Out.WriteLine($"Test cancellation requested. Test {pause}");
                cts.Cancel();
            });
        }
        await Task.Delay(TimeSpan.FromSeconds(pause % 10),cts.Token).ConfigureAwait(false);
    }
}
