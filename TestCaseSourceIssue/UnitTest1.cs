namespace TestCaseSourceIssue;

public class Tests
{
    
    [TestCaseSource(nameof(SourceOne))]
    public void TestOne(string a, int b)
    {
        Assert.That(Convert.ToInt32(a),Is.EqualTo(b));
    }



    private static IEnumerable<TestCaseData> SourceOne()
    {
        string[] names = [ "One", "Two", "Three", "Four","Five" ];
        int i = 0;
        foreach (var name in names)
        {
            i++;
            yield return new TestCaseData(i.ToString(), i).SetName($"{nameof(TestOne)}.{name}");
        }


    }

    [TestCaseSource(nameof(SourceTwo))]
    public void TestTwo(KeyValuePair<string, int> kvp)
    {
        Assert.That(Convert.ToInt32(kvp.Key), Is.EqualTo(kvp.Value));
    }



    private static IEnumerable<KeyValuePair<string, int>> SourceTwo()
    {
        string[] names = ["One", "Two", "Three", "Four", "Five"];
        for (int i = 1; i < 6; i++)
        {
            var key = new KeyValuePair<string, int>(i.ToString(), i);
            yield return key;
        }


    }

}
