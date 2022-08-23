namespace NUnit4160;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Category("Whatever")]
    [Test]
    public void Test1()
    {
        Assert.Pass();
    }

     [Test]
    public void Test2()
    {
        Assert.Pass();
    }
    
    [Category("TCS")]
    [TestCaseSource(nameof(DivideCases))]
    public void DivideTest(int n, int d, int q)
    {
        Assert.That(n / d,Is.EqualTo(q));
    }

    static object[] DivideCases =
    {
        new object[] { 12, 3, 4 },
        new object[] { 12, 2, 6 },
        new object[] { 12, 4, 3 }
    };
}