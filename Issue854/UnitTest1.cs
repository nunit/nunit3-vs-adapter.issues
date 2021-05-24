using NUnit.Framework;

namespace Issue854
{
    public class Tests
    {
        [TestCaseSource(nameof(BUGSource))]
        public void Test_BUG((int, int)[] testcase) { }

        public static object[] BUGSource =
        {
            new object[] { new[] { (1, 1) } },
            new object[] { new[] { (2, 2) } },
            new object[] { new[] { (2, 2), (3, 3) } },
            new object[] { new[] { (2, 2), (3, 4) } },
            new object[] { new[] { (2, 2), (3, 3), (1, 1) } },
            new object[] { new[] { (2, 2), (3, 4), (1, 2) } },
        };
    }
}