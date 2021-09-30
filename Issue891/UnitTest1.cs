using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;

[assembly: Parallelizable(ParallelScope.Fixtures)]

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
            public List<string> AllLogs { get; set; } = new ();
            public void Log(string message)
            {
                AllLogs.Add(message);
            }
        }

        public abstract class BaseForLogging
        {
            public ILogger Logger { get; set; }

            [SetUp]
            public void Setup()
            {
                Logger = new NunitConsoleLogger();
            }
            
            [TearDown]
            public void TearDown()
            {
                foreach (var log in Logger.AllLogs)
                {
                    Console.WriteLine(log);
                }
            }
        }



        [Parallelizable(ParallelScope.Fixtures)]
        public class Tests : BaseForLogging
        {


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
        }
        
        [Parallelizable(ParallelScope.Fixtures)]
        public class Tests2 : BaseForLogging
        {

            [Test]
            public void Test5()
            {
                Thread.Sleep(3000);
                Logger.Log($"{TestContext.CurrentContext.Test.Name}  {Logger.GetHashCode()}");
            }


            
        }
    }

}