using NUnit.Framework;

namespace NUnitFilterSample;

[TestFixture]
internal class Tests
{
    [Explicit]
    [Test]
   public void Explicit()
    {
        Assert.Pass();
    }
    
    [Test]
    public void Test()
    {
        Assert.Pass();
    }
}