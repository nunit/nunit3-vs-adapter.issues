using System.Diagnostics;
using NUnit.Framework;

/// <summary>
/// A SetUpFixture outside of any namespace provides SetUp and TearDown for the entire assembly.
/// </summary>
[SetUpFixture]
public class GlobalSetUpFixture
{
    [OneTimeSetUp]
    public void OneTimeSetUp()
        => Trace.TraceInformation($"{nameof(GlobalSetUpFixture)} - tracing message from one time set up");

    [OneTimeTearDown]
    public void OneTimeTearDown()
        => Trace.TraceInformation($"{nameof(GlobalSetUpFixture)} - tracing message from one time teardown");
}

namespace Issue824
{
    /// <summary>
    /// PLEASE NOTE:
    /// The same is also true for inner namespace - it may contain no more that 1 SetUpFixture.
    /// </summary>
    namespace Inner
    {
        /// <summary>
        /// Multiple SetUpFixtures may be created in a given namespace. The order of execution of such fixtures is indeterminate.
        /// </summary>
        [SetUpFixture]
        public class NUnitTestsDemoInnerSetUpFixture1
        {
            [OneTimeSetUp]
            public void OneTimeSetUp() 
                => Trace.TraceInformation(nameof(NUnitTestsDemoInnerSetUpFixture1));

            [OneTimeTearDown]
            public void OneTimeTearDown() 
                => Trace.TraceInformation(nameof(NUnitTestsDemoInnerSetUpFixture1));
        }        
        
        /// <summary>
        /// Multiple SetUpFixtures may be created in a given namespace. The order of execution of such fixtures is indeterminate.
        /// </summary>
        [SetUpFixture]
        public class NUnitTestsDemoInnerSetUpFixture2
        {
            [OneTimeSetUp]
            public void OneTimeSetUp() 
                => Trace.TraceInformation(nameof(NUnitTestsDemoInnerSetUpFixture2));

            [OneTimeTearDown]
            public void OneTimeTearDown() 
                => Trace.TraceInformation(nameof(NUnitTestsDemoInnerSetUpFixture2));
        }

        public class InnerTests
        {
            [Test]
            public void Test() => Assert.That(true, "Passes");
        }
    }

    /// <summary>
    /// Multiple SetUpFixtures may be created in a given namespace. The order of execution of such fixtures is indeterminate.
    /// </summary>
    [SetUpFixture]
    public class NUnitTestsDemoSetUpFixture1
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
            => Trace.TraceInformation(nameof(NUnitTestsDemoSetUpFixture1));

        [OneTimeTearDown]
        public void OneTimeTearDown()
            => Trace.TraceInformation(nameof(NUnitTestsDemoSetUpFixture1));
    }

    /// <summary>
    /// Multiple SetUpFixtures may be created in a given namespace. The order of execution of such fixtures is indeterminate.
    /// </summary>
    [SetUpFixture]
    public class NUnitTestsDemoSetUpFixture2
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
            => Trace.TraceInformation(nameof(NUnitTestsDemoSetUpFixture2));

        [OneTimeTearDown]
        public void OneTimeTearDown()
            => Trace.TraceInformation(nameof(NUnitTestsDemoSetUpFixture2));
    }

    public class Tests
    {
        [Test]
        public void Test() => Assert.That(true, "Passes");
    }
}
