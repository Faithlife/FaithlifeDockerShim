using System.IO;

namespace Faithlife.DockerShim.Logging
{
	/// <inheritdoc/>
	/// This type writes all string messages to a text writer, without any escaping. This type is threadsafe if its underlying <see cref="TextWriter"/> is threadsafe.
	internal sealed class TextWriterStringLog : IStringLog
	{
		/// <summary>
		/// Creates a string logger that logs to a text writer.
		/// </summary>
		/// <param name="textWriter">The text writer.</param>
		public TextWriterStringLog(TextWriter textWriter)
		{
			m_textWriter = textWriter;
		}

		/// <inheritdoc/>
		public void WriteLine(string message) => m_textWriter.WriteLine(message);

		private readonly TextWriter m_textWriter;
	}
}
