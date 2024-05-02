using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace MyApp.Tests.NUnit
{
    public class UnitTests
    {
        [TestCase(TestName = "Main method returns zero exit code if called with single argument")]
        public void Main_SingleArgument_ReturnsZeroExitCode()
        {
            int exitCode = Program.Main(new string[] { "John" });
            Assert.That(exitCode, Is.EqualTo(0));
        }

        [TestCase(TestName = "Main method returns non-zero exit code if called with no arguments")]
        public void Main_NoArguments_ReturnsNonZeroExitCode()
        {
            int exitCode = Program.Main(Array.Empty<string>());
            Assert.That(exitCode, Is.Not.EqualTo(0));
        }

        [TestCase(TestName = "Main method returns zero exit code if called with multiple arguments")]
        public void Main_MultipleArguments_ReturnsZeroExitCode()
        {
            int exitCode = Program.Main(new string[] { "John", "42" });
            Assert.That(exitCode, Is.EqualTo(0));
        }
    }
}