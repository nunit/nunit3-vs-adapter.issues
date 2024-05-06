using ConsoleRecorder.PoC.Environment;
using NUnit.Framework.Constraints;

namespace ConsoleRecorder.PoC
{
    public class SystemTests
    {
        [TestCase(TestName = "Warning is output if called with multiple arguments")]
        public void Program_MultipleArguments_OutputsWarning()
        {
            const string ignoredArgsWarning = "Ignoring excess arguments";

            ConsoleOutput.Recorder.Start();
            Program.Main(new string[] { "John", "42" });
            ConsoleOutput.Recorder.Stop();

            string errorOutput = ConsoleOutput.Error.ReadToEnd();
            Assert.That(errorOutput, Contains.Substring(ignoredArgsWarning));

            ConsoleOutput.Recorder.Reset();
        }

        [TestCase(TestName = "Warning is output before greeting if called with multiple arguments")]
        public void Program_MultipleArguments_WarningBeforeGreeting()
        {
            const string warning = "Warning:";
            const string greeting = "Hello";

            ConsoleOutput.Recorder.Start();
            Program.Main(new string[] { "John", "42" });
            ConsoleOutput.Recorder.Stop();

            var output = ConsoleOutput.All;
            string? line = output.ReadLine();
            Assert.That(line, Is.Not.Null);
            Assert.That(line!.StartsWith(warning));
            line = output.ReadLine();
            Assert.That(line, Is.Not.Null);
            Assert.That(line!.Contains(greeting));

            ConsoleOutput.Recorder.Reset();
        }

        [TestCase(TestName = "Test case with suppressed console output")]
        public void Program_NoConsoleOutput()
        {
            ConsoleOutput.Disable();
            Console.Out.WriteLine("This output will be suppressed!");
            Console.Error.WriteLine("And so will be this!");
            ConsoleOutput.Enable();

            Program.Main(new string[] { "John", "42" });
            Assert.Pass();
        }
    }
}