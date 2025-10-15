namespace Issue1111;

#if IGNORE_FOR_NOW
public class Tests
{
    [Category("IsExplicit")]
    [Explicit]
    [Test]
    public void Test1()
    {
        Assert.Pass();
    }
}
#else
public class Tests
{

    [Test, Explicit, Category("FooGroup"), Category("AllGroup")]
    public void Foo()
    {
        Assert.Pass();
    }

    [Test, Explicit, Category("IsExplicit"), Category("FooGroup"), Category("AllGroup")]
    public void FooExplicit()
    {
        Assert.Pass();
    }

    [Test, Explicit, Category("BarGroup"), Category("AllGroup")]
    public void Bar()
    {
        Assert.Pass();
    }
}
#endif
