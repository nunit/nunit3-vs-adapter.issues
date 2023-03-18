namespace Issue1041;

class Tests
  {

      /// <summary>
      ///  To ensure our filters are actually working, we dont want this test out
      /// </summary>
      [Test]
      public void SomeOtherTest()
      {
        TestContext.WriteLine("This is some other test.");
        Assert.Pass();
      }


    [TestCaseSource(nameof(GetTestCases))]
    public void RegularTest()
    { 
        TestContext.WriteLine("This is a regular test.");
    }

    [Category("ExplicitTests")]
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

    static List<TestCaseData> GetTestCasesNormal()
    {
        return new List<TestCaseData>
        {
            new TestCaseData(),
            new TestCaseData() 
        };
    }
}