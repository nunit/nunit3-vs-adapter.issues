using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace MyApp.Tests.MSTest
{
    [TestClass]
    public class UnitTests
    {
        [TestMethod("Main method returns zero exit code if called with single argument")]
        public void Main_SingleArgument_ReturnsZeroExitCode()
        {
            int exitCode = Program.Main(new string[] { "John" });
            Assert.AreEqual(exitCode, 0);
        }

        [TestMethod("Main method returns non-zero exit code if called with no arguments")]
        public void Main_NoArguments_ReturnsNonZeroExitCode()
        {
            int exitCode = Program.Main(Array.Empty<string>());
            Assert.AreNotEqual(exitCode, 0);
        }

        [TestMethod("Main method returns zero exit code if called with multiple arguments")]
        public void Main_MultipleArguments_ReturnsZeroExitCode()
        {
            int exitCode = Program.Main(new string[] { "John", "42" });
            Assert.AreEqual(exitCode, 0);
        }
    }
}