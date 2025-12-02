namespace VSTestAdapterArgNameCloseBraceBugRepro
{
    public class Tests
    {
        private static IEnumerable<TestCaseData> ArgDisplayNamesTestCaseSource()
        {
            yield return new TestCaseData(0).SetArgDisplayNames("AAAA");
            yield return new TestCaseData(1).SetArgDisplayNames("AAAA(AAAA");
            yield return new TestCaseData(2).SetArgDisplayNames("AAAA)AAAA");
            yield return new TestCaseData(3).SetArgDisplayNames("AAAA(AAAA)AAAA");
        }
        private static IEnumerable<TestCaseData> TestCaseDataToStringTestCaseSource()
        {
            yield return new TestCaseData(new FileInfo("AAAA"));
            yield return new TestCaseData(new FileInfo("AAAA(AAAA"));
            yield return new TestCaseData(new FileInfo("AAAA)AAAA"));
            yield return new TestCaseData(new FileInfo("AAAA(AAAA)AAAA"));
        }
        private static IEnumerable<FileInfo> FileInfoToStringTestCaseSource()
        {
            yield return new FileInfo("AAAA");
            yield return new FileInfo("AAAA(AAAA");
            yield return new FileInfo("AAAA)AAAA");
            yield return new FileInfo("AAAA(AAAA)AAAA");
        }
        private static IEnumerable<TestCaseData> QuotedArgDisplayNamesTestCaseSource()
        {
            yield return new TestCaseData(0).SetArgDisplayNames("\"AAAA\"");
            yield return new TestCaseData(1).SetArgDisplayNames("\"AAAA(AAAA\"");
            yield return new TestCaseData(2).SetArgDisplayNames("\"AAAA)AAAA\"");
            yield return new TestCaseData(3).SetArgDisplayNames("\"AAAA(AAAA)AAAA\"");
            yield return new TestCaseData(4).SetArgDisplayNames("\"Files\\A.zip\"");
            yield return new TestCaseData(5).SetArgDisplayNames("\"Files\\AAAA(AAAA.zip\"");
            yield return new TestCaseData(6).SetArgDisplayNames("\"Files\\AAAA)AAAA.zip\"");
            yield return new TestCaseData(7).SetArgDisplayNames("\"Files\\AAAA(AAAA)AAAA.zip\"");
            yield return new TestCaseData(9).SetArgDisplayNames("\"Files\\A(2.3).zip\"");
            yield return new TestCaseData(10).SetArgDisplayNames("\"Files\\AAAA(AAAA).zip\"");
            yield return new TestCaseData(11).SetArgDisplayNames("\"Files\\AAAA)AAAA).zip\"");
            yield return new TestCaseData(12).SetArgDisplayNames("\"Files\\AAAA(AA.AA)AAAA.zip\"");
        }

        [TestCase("AAAA")]
        [TestCase("AAAA(AAAA")]
        [TestCase("AAAA)AAAA")]
        [TestCase("AAAA(AAAA)AAAA")]
        [TestCase("Files\\A(2.3).zip")]
        [TestCase("Files\\AAAA(AAAA).zip")]
        [TestCase("Files\\AAAA)AAAA).zip")]
        [TestCase("Files\\AAAA(AA.AA)AAAA.zip")]
        public void Test1(string name) => Assert.Pass();

        [TestCaseSource(nameof(ArgDisplayNamesTestCaseSource))]
        public void Test2(int value) => Assert.Pass();

        [TestCaseSource(nameof(TestCaseDataToStringTestCaseSource))]
        public void Test3(FileInfo value) => Assert.Pass();

        [TestCaseSource(nameof(FileInfoToStringTestCaseSource))]
        public void Test4(FileInfo value) => Assert.Pass();

        [TestCaseSource(nameof(QuotedArgDisplayNamesTestCaseSource))]
        public void Test5(int value) => Assert.Pass();
    }
}