namespace Issue996;

public class Tests
{
    [Category("MyTest")]
    [TestCase("1",(char)1)]
    public void Test1(object x, object y)
    {
        Assert.That(x,Is.EqualTo(y));
    }

    //[Test]
    //public void Test2()
    //{
    //    Assert.That(1, Is.EqualTo(1));
    //}
}