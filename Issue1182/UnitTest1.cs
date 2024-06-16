
using System.Collections;
using System.Runtime.CompilerServices;

namespace TestOfCategories;

public class MyTestCase(string name, int someNumber, string someString)
{
    public int SomeNumber { get; } = someNumber;
    public string SomeString { get; } = someString;

    public string DataName { get; } = name;
}

public class MyTestCaseData : TestCaseData
{
    public MyTestCaseData(MyTestCase testCase) : base(testCase)
    {
        SetName($"{testCase.DataName} {testCase.SomeNumber} {testCase.SomeString} ");
        SetCategory(testCase.DataName);
    }
}

public class SimpleTestData_FirstDataSource
{
    public static IEnumerable GetTestCases
    {
        get
        {
            yield return new MyTestCaseData(new MyTestCase(nameof(SimpleTestData_FirstDataSource), 1, "Whatever 1"));
            yield return new MyTestCaseData(new MyTestCase(nameof(SimpleTestData_FirstDataSource), 2, "Whatever 2"));
            yield return new MyTestCaseData(new MyTestCase(nameof(SimpleTestData_FirstDataSource), 3, "Whatever 3"));
        }
    }
}

public class SimpleTestData_SecondDataSource
{
    public static IEnumerable GetTestCases
    {
        get
        {
            yield return new MyTestCaseData(new MyTestCase(nameof(SimpleTestData_SecondDataSource), 1, "Whatever 1"));
            yield return new MyTestCaseData(new MyTestCase(nameof(SimpleTestData_SecondDataSource), 2, "Whatever 2"));
            yield return new MyTestCaseData(new MyTestCase(nameof(SimpleTestData_SecondDataSource), 3, "Whatever 3"));
        }
    }
}

public class SimpleTestData_ThirdDataSource
{
    public static IEnumerable GetTestCases
    {
        get
        {
            yield return new MyTestCaseData(new MyTestCase("ThirdDataSource_Test1", 1, "One"));
            yield return new MyTestCaseData(new MyTestCase("ThirdDataSource_Test1", 2, "Two"));
        }
    }
}



public class Tests
{
    [Test]
    [TestCaseSource(typeof(SimpleTestData_FirstDataSource), nameof(SimpleTestData_FirstDataSource.GetTestCases))]
    [TestCaseSource(typeof(SimpleTestData_SecondDataSource), nameof(SimpleTestData_SecondDataSource.GetTestCases))]
    [TestCaseSource(typeof(SimpleTestData_ThirdDataSource), nameof(SimpleTestData_ThirdDataSource.GetTestCases))]
    public void SimpleTest(MyTestCase data)
    {
        Console.WriteLine(data);
    }
}


