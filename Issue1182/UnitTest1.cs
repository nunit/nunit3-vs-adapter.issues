
using NUnit.Framework.Interfaces;
using System.Collections;

namespace TestOfCategories;

public abstract class TestData(string dataName) 
{
    public string DataName => dataName;

	public override string ToString()
	{
		return DataName;
	}

}

public class SimpleTestData(string dataName) : TestData(dataName)
{
	public int? SomeNumber;
	public string? SomeString;
}

public class Tests
{
	[Test]
	[TestCaseSource(typeof(SimpleTestData_FirstDataSource), Category = "FirstDataSource")]
	[TestCaseSource(typeof(SimpleTestData_SecondDataSource), Category = "SecondDataSource")]
	[TestCaseSource(typeof(SimpleTestData_ThirdDataSource), Category = "ThirdDataSource")]
	public void SimpleTest(SimpleTestData data)
	{
		Console.WriteLine(data);
	}
}

public class SimpleTestData_FirstDataSource : IEnumerable
{
	public IEnumerator GetEnumerator()
	{
		yield return new SimpleTestData("Test1") { SomeNumber = 1, SomeString = "One" };//.SetName("FirstDataSource_Test1");
		yield return new SimpleTestData("Test2") { SomeNumber = 2, SomeString = "Two" };//.SetName("FirstDataSource_Test2");
	}
}

public class SimpleTestData_SecondDataSource : IEnumerable
{
	public IEnumerator GetEnumerator()
	{
		//SecondDataSource is not visible in tests, only because it has same DataNames as FirstDataSource (Test1, Test2)
		yield return new SimpleTestData("Test1") { SomeNumber = 1, SomeString = "One" };//.SetName("SecondDataSource_Test1");
        yield return new SimpleTestData("Test2") { SomeNumber = 2, SomeString = "Two" };//.SetName("SecondDataSource_Test2");

		//As soon as we change DataNames, SecondDataSource will be visible in tests, just uncomment these lines:

		//yield return new SimpleTestData("SecondDataSource_Test1") { SomeNumber = 1, SomeString = "One" };
		//yield return new SimpleTestData("SecondDataSource_Test2") { SomeNumber = 2, SomeString = "Two" };
	}
}

public class SimpleTestData_ThirdDataSource : IEnumerable
{
	public IEnumerator GetEnumerator()
	{
		yield return new SimpleTestData("ThirdDataSource_Test1") { SomeNumber = 1, SomeString = "One" };//.SetName("ThirdDataSource_Test1");
        yield return new SimpleTestData("ThirdDataSource_Test2") { SomeNumber = 2, SomeString = "Two" };//.SetName("ThirdDataSource_Test2");
	}
}