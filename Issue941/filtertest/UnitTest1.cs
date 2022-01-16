namespace filtertest;

public class Tests
{
        [Test]
    public void Test1()
    {
        Assert.Pass();
    }
}

public class Tests2
                    {
        
                        [Test,Category("FooGroup"),Category("AllGroup")]
                        public void Foo()
                        {
                            Assert.Pass();
                        }

                        [Test,Explicit,Category("IsExplicit"),Category("FooGroup"),Category("AllGroup")]
                        public void FooExplicit()
                        {
                            Assert.Pass();
                        }

                        [Test, Category("BarGroup"),Category("AllGroup")]
                        public void Bar()
                        {
                            Assert.Pass();
                        }
                    }