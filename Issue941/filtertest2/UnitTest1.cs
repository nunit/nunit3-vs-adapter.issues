namespace filtertest;

public class Tests
{
        [Test]
    public void Test21()
    {
        TestContext.WriteLine($"Test21 running");
        Assert.Pass();
    }
}

public class Tests22
                    {
        
                        [Test,Category("FooGroup"),Category("AllGroup")]
                        public void Foo()
                        {
                            TestContext.WriteLine($"{nameof(Tests22.Foo)} running");
                            Assert.Pass();
                        }

                        [Test,Explicit,Category("IsExplicit"),Category("FooGroup"),Category("AllGroup")]
                        public void FooExplicit()
                        {
                            TestContext.WriteLine($"{nameof(Tests22.FooExplicit)} running");
                            Assert.Pass();
                        }

                        [Test, Category("BarGroup"),Category("AllGroup")]
                        public void Bar()
                        {
                            TestContext.WriteLine($"{nameof(Tests22.Bar)} running");
                            Assert.Pass();
                        }
                    }