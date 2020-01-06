using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace nUnitExpressionTest.Tests
{
	/// <summary>
	/// See https://github.com/nunit/nunit3-vs-adapter/issues/622#issuecomment-552335787
	/// </summary>
	public class TupleUnitTests
	{
		//[Test]
		[TestCase(typeof(IDummy<(String body, Dictionary<String, String> applicationData), String>))]
		public void UnitTest_TestCaseWithTuple_TestIsNotExecuted(Type targetType)
		{
			Assert.That(targetType, Is.Not.Null);
		}

		//[Test]
		//[TestCase(typeof(IDummy<String, (String body, Dictionary<String, String> applicationData)>))]
		//public void UnitTest_TestCaseWithTuple_TestIsExecuted(Type targetType)
		//{
		//	Assert.That(targetType, Is.Not.Null);
		//}
	}

	public interface IDummy<T1, T2> : IList<T1> { }
	
}
