using NSubstitute;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

using Assert = NUnit.Framework.Legacy.ClassicAssert;

namespace NSubstituteSkippedTestDemo
{
    public abstract class TestsBase
    {
        protected static class Data
        {
            public static int Number { get; } = 5;
            public static string NumberAsText { get; } = "5";
        }

        protected static IThing GetInstance(bool mock)
        {
            IThing GetMockedInstance()
            {
                IThing thing = Substitute.For<IThing>();
                thing.GetNumberAsTextAsync(Data.Number).Returns(Task.FromResult(Data.NumberAsText));
                return thing;
            }

            return mock ? GetMockedInstance() : new Thing();
        }
    }

    #region TestsA

    public class TestsA : TestsBase
    {
        [Test]
        public async Task TestA1Async()
        {
            IThing thing = GetInstance(false);
            string result = await thing.GetNumberAsTextAsync(Data.Number);
            Assert.AreEqual(Data.NumberAsText, result);
        }

        [Test]
        public async Task TestA2Async()
        {
            IThing thing = GetInstance(true);
            string result = await thing.GetNumberAsTextAsync(Data.Number);
            Assert.AreEqual(Data.NumberAsText, result);
        }
    }

    #endregion TestsA

    #region TestsB

    public abstract class TestsBBase : TestsBase
    {
        public static IEnumerable<IThing> GetThingInstance(bool mock)
        {
            yield return GetInstance(mock);
        }
    }

    public class TestsB : TestsBBase
    {
        [TestCaseSource(nameof(GetThingInstance), new object[] { false })]
        public static async Task TestB1Async(IThing thing)
        {
            string result = await thing.GetNumberAsTextAsync(Data.Number);
            Assert.AreEqual(Data.NumberAsText, result);
        }

        [TestCaseSource(nameof(GetThingInstance), new object[] { true })]
        public static async Task TestB2Async(IThing thing)
        {
            string result = await thing.GetNumberAsTextAsync(Data.Number);
            Assert.AreEqual(Data.NumberAsText, result);
        }
    }

    #endregion TestsB
}
