namespace NUnitBugRepro
{
    public class Tests
    {
        [TestCaseSource(nameof(MinMaxValuesWithTuples))]
        public void MinMaxValueTestWithTuples((int Value, (int x, int y) MinValue, (int x, int y) MaxValue) _)
        {
            Assert.Pass();
        }

        private static IEnumerable<(int Value, (int x, int y) MinValue, (int x, int y) MaxValue)> MinMaxValuesWithTuples()
        {
            yield return (1, (-9, -9), (9, 9));
            yield return (2, (-99, -99), (99, 99));
        }

        [TestCaseSource(nameof(MinMaxValuesWithoutTuples))]
        public void MinMaxValueTestWithoutTuple((int Value, int MinValue, int MaxValue) _)
        {
            Assert.Pass();
        }

        private static IEnumerable<(int Value, int MinValue, int MaxValue)> MinMaxValuesWithoutTuples()
        {
            yield return (1, -9, 9);
            yield return (2, -99, 99);
        }
    }
}
