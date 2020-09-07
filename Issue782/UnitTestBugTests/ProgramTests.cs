using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

namespace UnitTestBug.Tests
{
	[TestFixture]
	public class ProgramTests
	{
		[TestCaseSource(typeof(TupleTestsCases))]
		public void TupleTests(IEnumerable<(object, object)> args)
		{
			Assert.Fail();
		}

		private class TupleTestsCases : IEnumerable
		{
			public IEnumerator GetEnumerator()
			{
				yield return new TestCaseData(new[] { (new object(), new object()) });
			}
		}
	}
}