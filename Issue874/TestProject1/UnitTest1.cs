using NUnit.Framework;

namespace TestProject1
{
	public class Tests
	{

		[Category("A")]
		[Test]
		[Explicit]
		public void Test1()
		{
			Assert.Fail();
		}

		[Category("A")]
        [Test]
    	public void Test2()
        {
            Assert.Pass();
        }

		[Test, Category("B")]
    	public void Test3()
        {
            Assert.Pass();
        }
    }
}