namespace ConsoleRecorder.PoC
{
    /// <summary>
    /// Provides mechanisms for recording data.
    /// </summary>
    public interface IRecorder
    {
        /// <summary>
        /// Gets a value indicating whether a recording is running.
        /// </summary>
        bool IsRecording { get; }

        /// <summary>
        /// Starts the recording.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the recording.
        /// </summary>
        void Stop();

        /// <summary>
        /// Erases the recorded data and prepares the recorder for a new recording.
        /// </summary>
        void Reset();
    }
}
