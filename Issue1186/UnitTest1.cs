namespace Issue1186;

public class Tests
{
    [Test]
    public void OneTest()
    {
        Console.WriteLine("All is good");
    }

    [Test]
    [Explicit("A reason goes here")]
    public void ExplicitTest()
    {

    }

    [Test]
    [Category("Foo")]
    public void AnotherTest()
    {
        Console.WriteLine("All is good");
    }   

}