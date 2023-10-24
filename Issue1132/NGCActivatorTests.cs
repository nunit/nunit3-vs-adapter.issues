using Autofac;
using FakeItEasy;
using NUnit.Framework;
using NUnit.Framework.Internal;
using NUnitLite;

namespace TestCase
{
    public interface INGCInstance
    { 

    }

    public class TestComponent : INGCInstance
    {
        public string ComponentId => nameof(ComponentId);

        public string CompName => nameof(CompName);

        public void Start()
        {
        }
    }

    public class TestComponentA : TestComponent
    { }

    public class TestComponentB : TestComponent
    { }



    public class TestModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType(typeof(NGCActivatorTests));
        }
    }

    internal static class Program
    {
        public static int Main(string[] args)
        {
            return new AutoRun().Execute(args);
        }
    }


    [TestFixture]
    public class NGCActivatorTests
    {
        public NGCActivatorTests()
        {
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {

        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
        }

        [SetUp]
        public void SetUp()
        {
        }

        [TearDown]
        public void TearDown()
        {
        }

        [TestCase]
        public void Test_ModuleRegistration()
        {
            Assert.IsTrue(true); 
        }

        [TestCase]
        public void Test_ComponentRegistration_Default()
        {
            Assert.IsTrue(true);
        }

        [TestCase]
        public void Test_ComponentRegistration_ConfigurationA()
        {
            Assert.IsTrue(true);
        }

        [TestCase]
        public void Test_ComponentRegistration_ConfigurationB()
        {
            Assert.IsTrue(true);
        }
    }
}
