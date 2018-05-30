using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Faithlife.Utility;
using Microsoft.Extensions.Logging;

namespace Faithlife.DockerShim.Logging
{
	/// <inheritdoc />
	/// <summary>
	/// The logger provider used by DockerShim. Sends logs to an <see cref="IStringLog"/>.
	/// </summary>
	internal sealed partial class DockerShimLoggerProvider : ILoggerProvider
	{
		/// <summary>
		/// Creates a new logger provider.
		/// </summary>
		/// <param name="stringLog">The underlying string log to which all logs are written. May not be <c>null</c>.</param>
		/// <param name="formatter">The formatter used to translate log events into single-line strings. May not be <c>null</c>.</param>
		/// <param name="filter">The filter for determining which log events to log. May not be <c>null</c>.</param>
		public DockerShimLoggerProvider(IStringLog stringLog, Func<LogEvent, string> formatter, Func<string, LogLevel, bool> filter)
		{
			if (stringLog == null)
				throw new ArgumentNullException(nameof(stringLog));
			if (formatter == null)
				throw new ArgumentNullException(nameof(formatter));
			if (filter == null)
				throw new ArgumentNullException(nameof(filter));

			m_stringLog = stringLog;
			m_formatter = formatter;
			m_filter = filter;
			m_loggers = new ConcurrentDictionary<string, ILogger>();
			m_scopes = new AsyncLocal<ImmutableStack<object>>();
		}

		/// <inheritdoc/>
		public ILogger CreateLogger(string categoryName)
		{
			if (categoryName == null)
				throw new ArgumentNullException(nameof(categoryName));
			return m_loggers.GetOrAdd(categoryName, name => new DockerShimLoggerProvider.DockerShimLogger(this, name));
		}

		void IDisposable.Dispose() { }

		private void Log(string loggerName, LogLevel logLevel, EventId eventId, string message, Exception exception,
			IEnumerable<KeyValuePair<string, object>> state)
		{
			if (message == "" && exception == null)
				return;
			var scopes = Scopes.Reverse();
			var text = m_formatter(new LogEvent
			{
				LoggerName = loggerName,
				LogLevel = logLevel,
				EventId = eventId,
				Message = message,
				Exception = exception,
				State = state,
				Scope = scopes,
			});
			m_stringLog.WriteLine(text);
		}

		private bool IsEnabled(string loggerName, LogLevel logLevel) => m_filter(loggerName, logLevel);

		private IDisposable BeginScope<TState>(TState state)
		{
			var previousScopes = Scopes;
			Scopes = previousScopes.Push(state);
			return Scope.Create(() => Scopes = previousScopes);
		}

		private ImmutableStack<object> Scopes
		{
			get => m_scopes.Value ?? ImmutableStack<object>.Empty;
			set => m_scopes.Value = value.IsEmpty ? null : value;
		}

		private readonly IStringLog m_stringLog;
		private readonly Func<LogEvent, string> m_formatter;
		private readonly Func<string, LogLevel, bool> m_filter;
		private readonly ConcurrentDictionary<string, ILogger> m_loggers;
		private readonly AsyncLocal<ImmutableStack<object>> m_scopes;
	}
}
