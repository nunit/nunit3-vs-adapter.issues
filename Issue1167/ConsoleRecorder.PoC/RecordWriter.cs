using System.Text;

namespace ConsoleRecorder.PoC
{
    /// <summary>
    /// Implements a <see cref="TextWriter"/> that records data for an <see cref="IRecorder"/>.
    /// </summary>
    /// <remarks>This writer does not implement a backing store.</remarks>
    internal class RecordWriter : TextWriter
    {
        /// <summary>
        /// The recorder that controls the recording.
        /// </summary>
        private readonly IRecorder _recorder;

        /// <summary>
        /// The record to which data is written.
        /// </summary>
        private readonly TextWriter _record;

        /// <inheritdoc/>
        public override Encoding Encoding => _record.Encoding;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordWriter"/> class.
        /// </summary>
        /// <param name="recorder">The recorder that controls the recording.</param>
        /// <param name="record">The record to which data is written.</param>
        /// <exception cref="ArgumentNullException">Thrown if any of the arguments is null.</exception>
        public RecordWriter(IRecorder recorder, TextWriter record)
        {
            ArgumentNullException.ThrowIfNull(recorder);
            _recorder = recorder;

            ArgumentNullException.ThrowIfNull(record);
            _record = record;
        }

        /// <inheritdoc/>
        public override void Write(char value)
        {
            if (_recorder.IsRecording)
                _record.Write(value);
        }
    }
}
