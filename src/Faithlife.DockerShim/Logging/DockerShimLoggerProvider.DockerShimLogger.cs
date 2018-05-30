using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Faithlife.DockerShim.Logging
{
	internal sealed partial class DockerShimLoggerProvider
	{
		private sealed class DockerShimLogger : ILogger
		{
			public DockerShimLogger(DockerShimLoggerProvider provider, string name)
			{
				m_provider = provider;
				m_name = name;
			}

			public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
				Func<TState, Exception, string> formatter)
			{
				if (!IsEnabled(logLevel))
					return;
				m_provider.Log(m_name, logLevel, eventId, formatter(state, exception) ?? "", exception,
					state as IEnumerable<KeyValuePair<string, object>>);
			}

			public bool IsEnabled(LogLevel logLevel) => m_provider.IsEnabled(m_name, logLevel);

			public IDisposable BeginScope<TState>(TState state) => m_provider.BeginScope(state);

			private readonly DockerShimLoggerProvider m_provider;
			private readonly string m_name;
		}
	}
}
