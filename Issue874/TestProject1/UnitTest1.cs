using NUnit.Framework;

namespace TestProject1
{
	public class Tests
	{
		[Test]
		[Explicit]
		public void Test1()
		{
			Assert.Fail();
		}

        [Test]
        public void Test2()
        {
            Assert.Pass();
        }
    }
}