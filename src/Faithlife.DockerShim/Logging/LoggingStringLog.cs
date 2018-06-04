using System;
using Microsoft.Extensions.Logging;

namespace Faithlife.DockerShim.Logging
{
	/// <summary>
	/// A string log that writes to an <see cref="ILogger"/>.
	/// </summary>
	internal sealed class LoggingStringLog : IStringLog
	{
		/// <summary>
		/// Creates a string log that writes to loggers provided by the given logger provider.
		/// </summary>
		/// <param name="loggerFactory">The logger provider.</param>
		/// <param name="stdoutParser">The parser to determine how to log messages.</param>
		public LoggingStringLog(ILoggerFactory loggerFactory, Action<string, ILoggerFactory> stdoutParser)
		{
			m_loggerFactory = loggerFactory;
			m_stdoutParser = stdoutParser;
		}

		/// <inheritdoc/>
		public void WriteLine(string message) => m_stdoutParser(message, m_loggerFactory);

		private readonly ILoggerFactory m_loggerFactory;
		private readonly Action<string, ILoggerFactory> m_stdoutParser;
	}
}
