using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace MyApp.Tests.xUnit
{
    public class UnitTests
    {
        [Fact(DisplayName = "Main method returns zero exit code if called with single argument")]
        public void Main_SingleArgument_ReturnsZeroExitCode()
        {
            int exitCode = Program.Main(new string[] { "John" });
            Assert.Equal(0, exitCode);
        }

        [Fact(DisplayName = "Main method returns non-zero exit code if called with no arguments")]
        public void Main_NoArguments_ReturnsNonZeroExitCode()
        {
            int exitCode = Program.Main(Array.Empty<string>());
            Assert.NotEqual(0, exitCode);
        }

        [Fact(DisplayName = "Main method returns zero exit code if called with multiple arguments")]
        public void Main_MultipleArguments_ReturnsZeroExitCode()
        {
            int exitCode = Program.Main(new string[] { "John", "42" });
            Assert.Equal(0, exitCode);
        }
    }
}