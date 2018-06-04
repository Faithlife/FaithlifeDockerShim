namespace Faithlife.DockerShim.Logging
{
	/// <summary>
	/// A string logger that backslash-escapes EOL characters before passing them to an inner logger. This type is threadsafe if its underlying <see cref="IStringLog"/> is threadsafe.
	/// </summary>
	internal sealed class EscapingStringLog : IStringLog
	{
		/// <summary>
		/// Creates a new escaping log wrapper around an existing log.
		/// </summary>
		/// <param name="log">The inner logger.</param>
		public EscapingStringLog(IStringLog log)
		{
			m_log = log;
		}

		/// <inheritdoc />
		public void WriteLine(string message) => m_log.WriteLine(Escaping.BackslashEscape(message));

		private readonly IStringLog m_log;
	}
}
