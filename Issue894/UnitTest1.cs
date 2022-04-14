using NUnit.Framework;



//[assembly:Explicit]
//[assembly:Category("TestMe")]

namespace nunit_explicit
{
    //[Explicit]
    [Category("TestMe")]
    public class Tests
    {
        // [Explicit]
        //[Category("TestMe")]
        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
        
        // [Category("TestMe")]
        //[Test]
        //public void Test2()
        //{
        //    Assert.Pass();
        //}
    }
}
