using NUnit.Framework;

namespace NUnitAdapterBug
{

	public class Tests
	{
		[SetUp]
		public void Setup()
		{
		}

		[Test]
		public void Test1()
		{
			Assert.Pass();
		}
	}
}
