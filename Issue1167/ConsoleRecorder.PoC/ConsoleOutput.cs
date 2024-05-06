using ConsoleRecorder.PoC.Utilities;

namespace ConsoleRecorder.PoC.Environment
{
    /// <summary>
    /// Provides services for interacting with the console output from the test host.
    /// This class cannot be inherited.
    /// </summary>
    internal static class ConsoleOutput
    {
        /// <summary>
        /// Gets a value indicating whether console output from the test host is enabled.
        /// </summary>
        public static bool IsEnabled { get; private set; }

        /// <summary>
        /// Returns a reader for the recorded standard output stream.
        /// </summary>
        public static TextReader Out => new StringReader(
            _recorder.GetRecording(ConsoleRecorder.ChannelSelection.Out));

        /// <summary>
        /// Returns a reader for the recorded standard error stream.
        /// </summary>
        public static TextReader Error => new StringReader(
            _recorder.GetRecording(ConsoleRecorder.ChannelSelection.Error));

        /// <summary>
        /// Returns a reader for the recorded console output.
        /// </summary>
        public static TextReader All => new StringReader(
            _recorder.GetRecording(
                ConsoleRecorder.ChannelSelection.Out | ConsoleRecorder.ChannelSelection.Error));

        /// <summary>
        /// Gets the interface to the console recorder.
        /// </summary>
        public static IRecorder Recorder => _recorder;

        /// <summary>
        /// A recorder for capturing console output.
        /// </summary>
        private static readonly ConsoleRecorder _recorder;
        
        /// <summary>
        /// The original standard output stream of the test host.
        /// </summary>
        private static readonly TextWriter _testHostStdOut;

        /// <summary>
        /// The original standard error stream of the test host.
        /// </summary>
        private static readonly TextWriter _testHostStdErr;

        /// <summary>
        /// Initializes static members of the <see cref="ConsoleOutput"/> class.
        /// </summary>
        static ConsoleOutput()
        {
            _recorder = new ConsoleRecorder();

            _testHostStdOut = Console.Out;
            _testHostStdErr = Console.Error;

            // Intercept the console output and split it between console and recorder.
            Console.SetOut(new DualWriter(_testHostStdOut, _recorder.Channels.Out));
            Console.SetError(new DualWriter(_testHostStdErr, _recorder.Channels.Error));
            IsEnabled = true;
        }

        /// <summary>
        /// Enables console output from the test host.
        /// </summary>
        public static void Enable()
        {
            if (IsEnabled)
                return;

            Console.SetOut(new DualWriter(_testHostStdOut, _recorder.Channels.Out));
            Console.SetError(new DualWriter(_testHostStdErr, _recorder.Channels.Error));

            IsEnabled = true;
        }

        /// <summary>
        /// Disables console output from the test host.
        /// </summary>
        public static void Disable()
        {
            if (!IsEnabled)
                return;

            Console.SetOut(new DualWriter(TextWriter.Null, _recorder.Channels.Out));
            Console.SetError(new DualWriter(TextWriter.Null, _recorder.Channels.Error));

            IsEnabled = false;
        }
    }
}
