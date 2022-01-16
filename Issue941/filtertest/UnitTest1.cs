namespace filtertest;

public class Tests
{
        [Test]
    public void Test1()
    {
        TestContext.WriteLine($"Tests1 running");
        
        Assert.Pass();
    }
}

public class Tests2
                    {
        
                        [Test,Category("FooGroup"),Category("AllGroup")]
                        public void Foo()
                        {
                            TestContext.WriteLine($"{nameof(Tests2.Foo)} running");
                            Assert.Pass();
                        }

                        [Test,Explicit,Category("IsExplicit"),Category("FooGroup"),Category("AllGroup")]
                        public void FooExplicit()
                        {
                            TestContext.WriteLine($"{nameof(Tests2.FooExplicit)} running");
                            Assert.Pass();
                        }

                        [Test, Category("BarGroup"),Category("AllGroup")]
                        public void Bar()
                        {
                            TestContext.WriteLine($"{nameof(Tests2.Bar)} running");
                            Assert.Pass();
                        }
                    }