namespace ConsoleRecorder.PoC
{
    using global::ConsoleRecorder.PoC.Utilities;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Text;

    /// <summary>
    /// Provides recording capabilities for console output.
    /// </summary>
    public class ConsoleRecorder : IRecorder
    {
        /// <summary>
        /// Selects the recorded channels.
        /// </summary>
        [Flags]
        public enum ChannelSelection
        {
            /// <summary>
            /// The recording of the standard output channel.
            /// </summary>
            Out = 0x1,

            /// <summary>
            /// The recording of the standard error channel.
            /// </summary>
            Error = 0x2
        }

        /// <summary>
        /// Gets a value indicating whether a recording is running.
        /// </summary>
        public bool IsRecording { get; private set; }

        internal RecorderChannels Channels { get; private set; }

        /// <summary>
        /// The list of recordings.
        /// </summary>
        /// <remarks>
        /// Retains individual recordings for each channel plus one combined recording.
        /// </remarks>
        private readonly StringWriter[] _recordings;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleRecorder"/> class.
        /// </summary>
        public ConsoleRecorder()
        {
            _recordings = Enumerable.Range(0, 3).Select(sw => new StringWriter()).ToArray();

            var outRecordWriter = new RecordWriter(
                recorder: this, record: new DualWriter(_recordings[0], _recordings[2]));
            var errorRecordWriter = new RecordWriter(
                recorder: this, record: new DualWriter(_recordings[1], _recordings[2]));

            Channels = new RecorderChannels()
            {
                Out = outRecordWriter,
                Error = errorRecordWriter
            };

            IsRecording = false;
        }

        /// <inheritdoc/>
        public void Start()
        {
            if (IsRecording)
            {
                throw new InvalidOperationException(
                    "Cannot start recording. A recording is already in progress. " +
                    "Stop the current recording before starting a new one.");
            }

            IsRecording = true;
        }

        /// <inheritdoc/>
        public void Stop()
        {
            if (!IsRecording)
            {
                throw new InvalidOperationException(
                    "Cannot stop recording. No recording is currently in progress. " +
                    "Start a recording before attempting to stop it.");
            }

            IsRecording = false;
        }

        /// <inheritdoc/>
        public void Reset()
        {
            if (IsRecording)
            {
                throw new InvalidOperationException(
                    "Cannot reset the recorder while a recording is in progress. " +
                    "Stop the current recording before resetting.");
            }

            foreach (var recording in _recordings)
                recording.GetStringBuilder().Clear();
        }

        /// <summary>
        /// Returns the recorded console output.
        /// </summary>
        /// <param name="recording">The type of recording.</param>
        /// <returns>A string containing the recorded console output.</returns>
        public string GetRecording(ChannelSelection channels)
        {
            if (channels.HasFlag(ChannelSelection.Out) && channels.HasFlag(ChannelSelection.Error))
                return _recordings[2].ToString();
            else if (channels.HasFlag(ChannelSelection.Out))
                return _recordings[0].ToString();
            else if (channels.HasFlag(ChannelSelection.Error))
                return _recordings[1].ToString();
            else
                return string.Empty;
        }
    }
}
