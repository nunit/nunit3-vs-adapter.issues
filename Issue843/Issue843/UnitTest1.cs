using System;
using System.Linq;
using NUnit.Framework;

namespace Issue843
{
    public class Tests
    {
        [Test]
		public void Sort_RandomData_IsSorted()
		{
			var random = TestContext.CurrentContext.Random;
			var data = Enumerable.Range(0, 2).Select(i => random.Next()).ToArray();

			Array.Sort(data);

			Assert.That(data, Is.Ordered);
		}

        [Test]
        public void Sort_RandomData_IsSorted2()
        {
            var random = TestContext.CurrentContext.Random;
            var data = Enumerable.Range(0, 2).Select(i => random.Next()).ToArray();

            Array.Sort(data);

            Assert.That(data, Is.Ordered);
        }
}
}