namespace Issue1041;

class Tests
  {
    [TestCaseSource(nameof(GetTestCases))]
    public void RegularTest()
    { 
        TestContext.WriteLine("This is a regular test.");
    }

    [TestCaseSource(nameof(GetTestCases))]
    [Explicit]
    public void ExplicitTest()
    { 
        TestContext.WriteLine("This is an explicit test.");
    }

    static List<TestCaseData> GetTestCases()
    {
      return new List<TestCaseData>
      {
        new TestCaseData() { TestName = $"The test number 1." },
        new TestCaseData() { TestName = $"The test number 2." }
      };
    }
  }