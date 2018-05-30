using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Faithlife.DockerShim.Logging
{
	/// <summary>
	/// An event to be logged.
	/// </summary>
	public sealed class LogEvent
	{
		/// <summary>
		/// The name (category) of the logger. Will not be <c>null</c>.
		/// </summary>
		public string LoggerName { get; set; }

		/// <summary>
		/// The importance of the event.
		/// </summary>
		public LogLevel LogLevel { get; set; }

		/// <summary>
		/// The id of the event, or <c>0</c> if there is no id.
		/// </summary>
		public EventId EventId { get; set; }

		/// <summary>
		/// The message. Will not be <c>null</c>, but may be the empty string.
		/// </summary>
		public string Message { get; set; }

		/// <summary>
		/// The exception, if any. May be <c>null</c>.
		/// </summary>
		public Exception Exception { get; set; }

		/// <summary>
		/// The structured state for the message, if any. May be <c>null</c>.
		/// </summary>
		public IEnumerable<KeyValuePair<string, object>> State { get; set; }

		/// <summary>
		/// The structured scope for the message, if any. Will not be <c>null</c>, but may be an empty sequence.
		/// </summary>
		public IEnumerable<object> Scope { get; set; }
	}
}
