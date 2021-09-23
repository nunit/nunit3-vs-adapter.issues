using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace GenericFixture
{
    //[SetUpFixture]
    //public class Setupfixture
    //{

    //}


    [TAAttribute]
    [TestFixture(typeof(ArrayList))]
    [TestFixture(typeof(List<int>))]
    public class ListTests<TList> where TList : IList, new()
    {
        private IList list;

        [SetUp]
        public void CreateList()
        {
            this.list = new TList();
        }

        [Test]
        public void CanAddToList()
        {
            list.Add(1); 
            list.Add(2); 
            list.Add(3);
            TestContext.WriteLine("In test");
            Assert.That(list.Count,Is.EqualTo(3));
        }
    }


    //public class TAAttribute : Attribute, ITestAction
    //{
    //    public void BeforeTest(ITest test)
    //    {
    //        TestContext.WriteLine("before");
    //    }

    //    public void AfterTest(ITest test)
    //    {
    //        TestContext.WriteLine("after");
    //    }

    //    public ActionTargets Targets => ActionTargets.Default;
    //}
}