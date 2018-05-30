using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Faithlife.DockerShim.Tests.Util
{
	public sealed class StubLoggerProvider : ILoggerProvider
	{
		public ImmutableQueue<LogMessage> Messages
		{
			get
			{
				lock (m_mutex)
					return m_messages ?? ImmutableQueue<LogMessage>.Empty;
			}
		}

		void IDisposable.Dispose() { }

		public ILogger CreateLogger(string categoryName) => new StubLogger(this, categoryName);

		private readonly object m_mutex = new object();
		private ImmutableQueue<LogMessage> m_messages;

		private void Log<TState>(string categoryName, LogLevel logLevel, EventId eventId, TState state, Exception exception,
			Func<TState, Exception, string> formatter)
		{
			var message = new LogMessage
			{
				CategoryName = categoryName,
				LogLevel = logLevel,
				EventId = eventId,
				State = state,
				Exception = exception,
				Message = formatter(state, exception) ?? "",
			};

			lock (m_mutex)
				m_messages = Messages.Enqueue(message);
		}

		private sealed class StubLogger : ILogger
		{
			public StubLogger(StubLoggerProvider stubLoggerProvider, string categoryName)
			{
				m_stubLoggerProvider = stubLoggerProvider;
				m_categoryName = categoryName;
			}

			public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
				Func<TState, Exception, string> formatter)
			{
				m_stubLoggerProvider.Log(m_categoryName, logLevel, eventId, state, exception, formatter);
			}

			public bool IsEnabled(LogLevel logLevel) => true;

			public IDisposable BeginScope<TState>(TState state) => null;

			private readonly StubLoggerProvider m_stubLoggerProvider;
			private readonly string m_categoryName;
		}

		public sealed class LogMessage
		{
			public string CategoryName { get; set; }
			public LogLevel LogLevel { get; set; }
			public EventId EventId { get; set; }
			public object State { get; set; }
			public Exception Exception { get; set; }
			public string Message { get; set; }
		}
	}
}
