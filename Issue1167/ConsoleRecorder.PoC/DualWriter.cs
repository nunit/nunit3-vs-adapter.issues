using System.Text;

namespace ConsoleRecorder.PoC.Utilities
{
    /// <summary>
    /// Implements a <see cref="TextWriter"/> for writing text to two writers in turn.
    /// </summary>
    /// <remarks>
    /// This writer does not implement a backing store.
    /// </remarks>
    internal class DualWriter : TextWriter
    {
        private readonly TextWriter _primaryWriter;
        private readonly TextWriter _secondaryWriter;

        /// <inheritdoc/>
        public override Encoding Encoding => Encoding.Default;

        /// <summary>
        /// Initializes a new instance of the <see cref="DualWriter"/> class.
        /// </summary>
        /// <param name="primaryWriter">The primary writer to write a copy to.</param>
        /// <param name="secondaryWriter">The secondary writer to write a copy to.</param>
        /// <exception cref="ArgumentNullException">Thrown if any of the arguments is null.</exception>
        public DualWriter(TextWriter primaryWriter, TextWriter secondaryWriter)
        {
            ArgumentNullException.ThrowIfNull(primaryWriter);
            ArgumentNullException.ThrowIfNull(secondaryWriter);

            _primaryWriter = primaryWriter;
            _secondaryWriter = secondaryWriter;
        }

        /// <inheritdoc/>
        public override void Write(char value)
        {
            _primaryWriter.Write(value);
            _secondaryWriter.Write(value);
        }

        /// <inheritdoc/>
        /// <remarks>
        /// This override exists as a workaround for the erroneous writing
        /// of single characters to the standard error stream when using NUnit.
        /// For more information, refer to <see href="https://github.com/nunit/nunit/issues/4414">issue #4414</see>.
        /// </remarks>
        public override void Write(string? value)
        {
            if (value is not null)
            {
                _primaryWriter.Write(value);
                _secondaryWriter.Write(value);
            }
        }

        /// <inheritdoc/>
        /// <remarks>
        /// This override exists as a workaround for the erroneous writing
        /// of single characters to the standard error stream when using NUnit.
        /// For more information, refer to <see href="https://github.com/nunit/nunit/issues/4414">issue #4414</see>.
        /// </remarks>
        public override void WriteLine(string? value)
        {
            if (value is not null)
            {
                _primaryWriter.WriteLine(value);
                _secondaryWriter.WriteLine(value);
            }
        }

        /// <inheritdoc/>
        public override void Flush()
        {
            _primaryWriter.Flush();
            _secondaryWriter.Flush();
        }
    }
}
