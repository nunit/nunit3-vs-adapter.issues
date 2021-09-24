using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;

//[assembly: Parallelizable(ParallelScope.All)]

namespace Issue891
{
    
   
    namespace Nunit3netcore
    {
        public interface ILogger
        {
            List<string> AllLogs { get; set; }
            void Log(string message);
        }

        public class NunitConsoleLogger : ILogger
        {
            public List<string> AllLogs { get; set; } = new List<string>();
            public void Log(string message)
            {
                AllLogs.Add(message);
            }
        }

       // [Parallelizable(ParallelScope.All)]
        public class Tests
        {
            public ILogger Logger { get; set; }

            [SetUp]
            public void Setup()
            {
                Logger = new NunitConsoleLogger();
            }

            [Test]
            public void Test1()
            {
                Thread.Sleep(3000);
                Logger.Log($"{TestContext.CurrentContext.Test.Name}  {Logger.GetHashCode()}");
            }

            [Test]
            public void Test2()
            {
                Thread.Sleep(3000);
                Logger.Log($"{TestContext.CurrentContext.Test.Name}  {Logger.GetHashCode()}");

            }

            [Test]
            public void Test5()
            {

                Thread.Sleep(3000);
                Logger.Log($"{TestContext.CurrentContext.Test.Name}  {Logger.GetHashCode()}");
            }


            [TearDown]
            public void TearDown()
            {
                foreach (var log in Logger.AllLogs)
                {
                    TestContext.WriteLine(log);
                }
                Logger = null;

            }
        }
    }

}